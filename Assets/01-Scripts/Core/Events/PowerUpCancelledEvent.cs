namespace AV.Framework.Core.Events
{
    using AV.Framework.Core.Gameplay;

    public readonly struct PowerUpCancelledEvent
    {
        public PowerUpCancelledEvent(PowerUpType powerUpType)
        {
            PowerUpType = powerUpType;
        }

        public PowerUpType PowerUpType { get; }
    }
}
