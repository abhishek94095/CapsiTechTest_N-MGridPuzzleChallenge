namespace AV.Framework.Core.Board
{
    public readonly struct PlayerDeathEvent
    {
        public PlayerDeathEvent(int pieceId)
        {
            PieceId = pieceId;
        }

        public int PieceId { get; }
    }
}