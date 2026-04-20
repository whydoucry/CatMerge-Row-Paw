using Microsoft.Xna.Framework;

namespace CatMergeRowPaw
{
    public class Board
    {
        public int Width { get; }
        public int Height { get; }
        public Tile?[,] Tiles { get; }
        public Cat?[,] Cats { get; }
        public bool[,] IsOpenCell { get; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile?[width, height];
            Cats = new Cat?[width, height];
            IsOpenCell = new bool[width, height];
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
