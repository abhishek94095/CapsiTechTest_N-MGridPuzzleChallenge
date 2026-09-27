namespace AV.Framework.Core.Events
{
    public readonly struct ObstacleCapturedEvent
    {
        public ObstacleCapturedEvent(int pieceId)
        {
            PieceId = pieceId;
        }

        public int PieceId { get; }
    }
}
