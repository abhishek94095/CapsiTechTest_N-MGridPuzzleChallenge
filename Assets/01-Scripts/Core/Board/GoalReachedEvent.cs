namespace AV.Framework.Core.Board
{
    public readonly struct GoalReachedEvent
    {
        public GoalReachedEvent(int playerPieceId)
        {
            PlayerPieceId = playerPieceId;
        }

        public int PlayerPieceId { get; }
    }
}