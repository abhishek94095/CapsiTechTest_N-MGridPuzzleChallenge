using System;
using AV.Framework.Core.Grid;
using MessagePipe;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace AV.Framework.Application
{
    public sealed class SwipeInputController : ITickable
    {
        private const float SwipeThreshold = 50f;

        private readonly IPublisher<GridDirection> publisher;
        private bool isPointerDown;
        private Vector2 pointerDownPosition;

        public SwipeInputController(IPublisher<GridDirection> publisher)
        {
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public void Tick()
        {
            if (TryGetKeyboardDirection(out GridDirection keyboardDirection))
            {
                Debug.Log($"Input detected: {keyboardDirection} (Keyboard)");
                publisher.Publish(keyboardDirection);
                return;
            }

            if (TryGetPointerDownPosition(out Vector2 downPosition))
            {
                isPointerDown = true;
                pointerDownPosition = downPosition;
                Debug.Log($"Pointer down: {pointerDownPosition}");
            }

            if (!isPointerDown) return;
            if (!TryGetPointerUpPosition(out Vector2 upPosition)) return;

            isPointerDown = false;

            Vector2 swipeVector = upPosition - pointerDownPosition;
            Debug.Log($"Pointer up: {upPosition}, Swipe: {swipeVector}");
            if (swipeVector.magnitude < SwipeThreshold)
            {
                Debug.Log("Input ignored: swipe below threshold.");
                return;
            }

            GridDirection direction = GetDirection(swipeVector);
            Debug.Log($"Input detected: {direction} (Swipe)");
            publisher.Publish(direction);
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

        private bool TryGetPointerDownPosition(out Vector2 position)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                position = Mouse.current.position.ReadValue();
                return true;
            }

            position = default;
            return false;
        }

        private bool TryGetPointerUpPosition(out Vector2 position)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                position = Mouse.current.position.ReadValue();
                return true;
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