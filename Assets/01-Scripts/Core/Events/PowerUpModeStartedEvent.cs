using AV.Framework.Core.Gameplay;

namespace AV.Framework.Core.Events
{
    public readonly struct PowerUpModeStartedEvent
    {
        public PowerUpType PowerUpType { get; }

        public PowerUpModeStartedEvent(PowerUpType powerUpType)
        {
            PowerUpType = powerUpType;
        }
    }
}
