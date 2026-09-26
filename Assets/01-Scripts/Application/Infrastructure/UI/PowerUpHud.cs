using System;
using AV.Framework.Core.Events;
using MessagePipe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private IDisposable powerUpStartedSubscription;
        private IDisposable powerUpEndedSubscription;

        public void Initialize(
            PowerUpController powerUpController,
            MessagePipe.ISubscriber<PowerUpModeStartedEvent> powerUpStartedSubscriber,
            MessagePipe.ISubscriber<PowerUpModeEndedEvent> powerUpEndedSubscriber)
        {
            this.powerUpController = powerUpController ?? throw new ArgumentNullException(nameof(powerUpController));
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

        private void Refresh()
        {
            if (powerUpController == null) return;

            hammerChargeText.text = powerUpController.HammerCharges.ToString();
            rocketChargeText.text = powerUpController.RocketCharges.ToString();

            bool isPowerUpActive = powerUpController.IsPowerUpActive;
            hammerButton.interactable = powerUpController.HammerCharges > 0 && !isPowerUpActive;
            rocketButton.interactable = powerUpController.RocketCharges > 0 && !isPowerUpActive;
            cancelButton.gameObject.SetActive(isPowerUpActive);
        }
    }
}
