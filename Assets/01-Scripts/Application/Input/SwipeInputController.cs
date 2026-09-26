using System;
using AV.Framework.Core.Board;
using AV.Framework.Core.Events;
using AV.Framework.Core.Gameplay;
using AV.Framework.Core.Grid;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace AV.Framework.Application
{
    public sealed class SwipeInputController : ITickable, IStartable, IDisposable
    {
        private const float SwipeThreshold = 50f;

        private readonly IPublisher<GridDirection> publisher;
        private readonly IPublisher<UndoRequestedEvent> undoPublisher;
        private readonly IPublisher<BoardTargetSelectedEvent> targetPublisher;
        private readonly ISubscriber<PowerUpModeStartedEvent> powerUpStartedSubscriber;
        private readonly ISubscriber<PowerUpModeEndedEvent> powerUpEndedSubscriber;
        private readonly BoardPresenter boardPresenter;
        private readonly Camera gameCamera;
        private IDisposable powerUpStartedSubscription;
        private IDisposable powerUpEndedSubscription;
        private bool isPointerDown;
        private bool isPowerUpModeActive;
        private bool pointerStartedOverUi;
        private int pointerId = -1;
        private Vector2 pointerDownPosition;
        private PowerUpType activePowerUpType;

        public SwipeInputController(
            IPublisher<GridDirection> publisher,
            IPublisher<UndoRequestedEvent> undoPublisher,
            IPublisher<BoardTargetSelectedEvent> targetPublisher,
            ISubscriber<PowerUpModeStartedEvent> powerUpStartedSubscriber,
            ISubscriber<PowerUpModeEndedEvent> powerUpEndedSubscriber,
            BoardPresenter boardPresenter,
            Camera gameCamera)
        {
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this.undoPublisher = undoPublisher ?? throw new ArgumentNullException(nameof(undoPublisher));
            this.targetPublisher = targetPublisher ?? throw new ArgumentNullException(nameof(targetPublisher));
            this.powerUpStartedSubscriber = powerUpStartedSubscriber ?? throw new ArgumentNullException(nameof(powerUpStartedSubscriber));
            this.powerUpEndedSubscriber = powerUpEndedSubscriber ?? throw new ArgumentNullException(nameof(powerUpEndedSubscriber));
            this.boardPresenter = boardPresenter ?? throw new ArgumentNullException(nameof(boardPresenter));
            this.gameCamera = gameCamera ?? throw new ArgumentNullException(nameof(gameCamera));
        }

        public void Start()
        {
            powerUpStartedSubscription = powerUpStartedSubscriber.Subscribe(OnPowerUpModeStarted);
            powerUpEndedSubscription = powerUpEndedSubscriber.Subscribe(OnPowerUpModeEnded);
        }

        public void Tick()
        {
            if (TryGetPointerDownPosition(out Vector2 downPosition, out int downPointerId))
            {
                pointerId = downPointerId;
                pointerDownPosition = downPosition;
                pointerStartedOverUi = IsPointerOverUi(downPointerId);
                isPointerDown = true;
            }

            if (Keyboard.current != null && Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (!isPowerUpModeActive) undoPublisher.Publish(new UndoRequestedEvent());
                return;
            }

            if (!isPowerUpModeActive && TryGetKeyboardDirection(out GridDirection keyboardDirection))
            {
                Debug.Log($"Input detected: {keyboardDirection} (Keyboard)");
                publisher.Publish(keyboardDirection);
                return;
            }

            if (isPowerUpModeActive && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                targetPublisher.Publish(new BoardTargetSelectedEvent(activePowerUpType, default, false));
                return;
            }

            if (!isPointerDown) return;
            if (!TryGetPointerUpPosition(out Vector2 upPosition, pointerId)) return;

            isPointerDown = false;
            bool pointerEndedOverUi = IsPointerOverUi(pointerId);
            if (pointerStartedOverUi || pointerEndedOverUi)
            {
                pointerStartedOverUi = false;
                return;
            }

            pointerStartedOverUi = false;
            Vector2 pointerVector = upPosition - pointerDownPosition;

            if (isPowerUpModeActive)
            {
                if (pointerVector.magnitude > SwipeThreshold) return;

                bool isValid = boardPresenter.TryGetGridPosition(upPosition, gameCamera, out GridPosition position);
                targetPublisher.Publish(new BoardTargetSelectedEvent(activePowerUpType, position, isValid));
                return;
            }

            if (pointerVector.magnitude < SwipeThreshold) return;
            publisher.Publish(GetDirection(pointerVector));
        }

        public void Dispose()
        {
            powerUpStartedSubscription?.Dispose();
            powerUpEndedSubscription?.Dispose();
        }

        private void OnPowerUpModeStarted(PowerUpModeStartedEvent powerUpModeStartedEvent)
        {
            isPowerUpModeActive = true;
            activePowerUpType = powerUpModeStartedEvent.PowerUpType;
            isPointerDown = false;
        }

        private void OnPowerUpModeEnded(PowerUpModeEndedEvent powerUpModeEndedEvent)
        {
            isPowerUpModeActive = false;
            isPointerDown = false;
            pointerStartedOverUi = false;
        }

        private static bool IsPointerOverUi(int currentPointerId)
        {
            if (EventSystem.current == null) return false;
            return currentPointerId >= 0
                ? EventSystem.current.IsPointerOverGameObject(currentPointerId)
                : EventSystem.current.IsPointerOverGameObject();
        }

        private bool TryGetKeyboardDirection(out GridDirection direction)
        {
            if (Keyboard.current == null)
            {
                direction = default;
                return false;
            }

            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                direction = GridDirection.Up;
                return true;
            }

            if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                direction = GridDirection.Down;
                return true;
            }

            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                direction = GridDirection.Left;
                return true;
            }

            if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                direction = GridDirection.Right;
                return true;
            }

            direction = default;
            return false;
        }

        private bool TryGetPointerDownPosition(out Vector2 position, out int currentPointerId)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                currentPointerId = Touchscreen.current.primaryTouch.touchId.ReadValue();
                return true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                position = Mouse.current.position.ReadValue();
                currentPointerId = -1;
                return true;
            }

            position = default;
            currentPointerId = -1;
            return false;
        }

        private bool TryGetPointerUpPosition(out Vector2 position, int currentPointerId)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                return currentPointerId == Touchscreen.current.primaryTouch.touchId.ReadValue();
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                position = Mouse.current.position.ReadValue();
                return currentPointerId == -1;
            }

            position = default;
            return false;
        }

        private GridDirection GetDirection(Vector2 swipeVector)
        {
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                return swipeVector.x > 0f ? GridDirection.Right : GridDirection.Left;
            }

            return swipeVector.y > 0f ? GridDirection.Up : GridDirection.Down;
        }
    }
}