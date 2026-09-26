using AV.Framework.Core.Gameplay;

namespace AV.Framework.Core.Events
{
    public readonly struct PowerUpModeEndedEvent
    {
        public PowerUpType PowerUpType { get; }

        public PowerUpModeEndedEvent(PowerUpType powerUpType)
        {
            PowerUpType = powerUpType;
        }
    }
}
