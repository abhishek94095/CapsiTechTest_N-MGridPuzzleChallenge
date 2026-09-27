using System;
using AV.Framework.Core.Events;
using MessagePipe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AV.Framework.Application
{
    public sealed class PowerUpHud : MonoBehaviour, IDisposable
    {
        [SerializeField] private Button hammerButton;
        [SerializeField] private Button rocketButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text hammerChargeText;
        [SerializeField] private TMP_Text rocketChargeText;

        private PowerUpController powerUpController;
        private GameFlow gameFlow;
        private IDisposable powerUpStartedSubscription;
        private IDisposable powerUpEndedSubscription;

        [Inject]
        public void Initialize(
            PowerUpController powerUpController,
            MessagePipe.ISubscriber<PowerUpModeStartedEvent> powerUpStartedSubscriber,
            MessagePipe.ISubscriber<PowerUpModeEndedEvent> powerUpEndedSubscriber,
            GameFlow gameFlow)
        {
            this.powerUpController = powerUpController ?? throw new ArgumentNullException(nameof(powerUpController));
            this.gameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            if (hammerButton == null) throw new InvalidOperationException("Hammer Button is not assigned.");
            if (rocketButton == null) throw new InvalidOperationException("Rocket Button is not assigned.");
            if (cancelButton == null) throw new InvalidOperationException("Cancel Button is not assigned.");
            if (hammerChargeText == null) throw new InvalidOperationException("Hammer Charge Text is not assigned.");
            if (rocketChargeText == null) throw new InvalidOperationException("Rocket Charge Text is not assigned.");

            hammerButton.onClick.AddListener(OnHammerClicked);
            rocketButton.onClick.AddListener(OnRocketClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
            powerUpStartedSubscription = powerUpStartedSubscriber.Subscribe(OnPowerUpModeStarted);
            powerUpEndedSubscription = powerUpEndedSubscriber.Subscribe(OnPowerUpModeEnded);
            gameFlow.StateChanged += OnGameFlowStateChanged;
            Refresh();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            hammerButton?.onClick.RemoveListener(OnHammerClicked);
            rocketButton?.onClick.RemoveListener(OnRocketClicked);
            cancelButton?.onClick.RemoveListener(OnCancelClicked);
            powerUpStartedSubscription?.Dispose();
            powerUpEndedSubscription?.Dispose();
            powerUpStartedSubscription = null;
            powerUpEndedSubscription = null;
            if (gameFlow != null) gameFlow.StateChanged -= OnGameFlowStateChanged;
        }

        private void OnHammerClicked()
        {
            powerUpController.ActivateHammer();
            Refresh();
        }

        private void OnRocketClicked()
        {
            powerUpController.ActivateRocket();
            Refresh();
        }

        private void OnCancelClicked()
        {
            powerUpController.CancelPowerUp();
            Refresh();
        }

        private void OnPowerUpModeStarted(PowerUpModeStartedEvent powerUpModeStartedEvent)
        {
            Refresh();
        }

        private void OnPowerUpModeEnded(PowerUpModeEndedEvent powerUpModeEndedEvent)
        {
            Refresh();
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (powerUpController == null) return;

            if (hammerChargeText != null) hammerChargeText.text = powerUpController.HammerCharges.ToString();
            if (rocketChargeText != null) rocketChargeText.text = powerUpController.RocketCharges.ToString();

            bool isPowerUpActive = powerUpController.IsPowerUpActive;
            bool isPlaying = gameFlow != null && gameFlow.State == GameFlowState.Playing;
            bool canActivate = isPlaying && !isPowerUpActive;

            if (hammerButton != null) hammerButton.interactable = canActivate && powerUpController.HammerCharges > 0;
            if (rocketButton != null) rocketButton.interactable = canActivate && powerUpController.RocketCharges > 0;
            if (cancelButton != null) cancelButton.gameObject.SetActive(isPlaying && isPowerUpActive);
            gameObject.SetActive(isPlaying);
        }
    }
}
