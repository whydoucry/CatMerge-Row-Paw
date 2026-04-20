using Microsoft.Xna.Framework;

namespace CatMergeRowPaw
{
    public enum TileType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
    }

    public class Tile
    {
        public TileType Type { get; set; }
        public bool IsSand { get; set; }

        public Tile(TileType type, bool isSand)
        {
            Type = type;
            IsSand = isSand;
        }

        public Color GetColor()
        {
            return Type switch
            {
                TileType.Red => new Color(240, 80, 80),
                TileType.Green => new Color(80, 220, 100),
                TileType.Blue => new Color(100, 160, 240),
                TileType.Yellow => new Color(240, 220, 100),
                TileType.Purple => new Color(170, 120, 240),
                _ => Color.White,
            };
        }
    }
}
