namespace AV.Framework.Core.Grid
{
    public readonly struct GridSize
    {
        public int Width { get; }
        public int Height { get; }
        public int CellCount => Width * Height;

        public GridSize(int width, int height)
        {
            if (width <= 0) throw new System.ArgumentOutOfRangeException(nameof(width));

            if (height <= 0) throw new System.ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
        }
    }
}
