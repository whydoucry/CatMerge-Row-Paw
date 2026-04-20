using Microsoft.Xna.Framework;

namespace CatMergeRowPaw
{
    public class Cat
    {
        public int Level { get; private set; }

        public Cat(int level)
        {
            Level = Math.Max(1, level);
        }

        public Cat Upgrade()
        {
            return new Cat(Level + 1);
        }

        public Color GetColor()
        {
            return Level switch
            {
                1 => new Color(230, 160, 130),
                2 => new Color(240, 200, 120),
                3 => new Color(180, 220, 190),
                4 => new Color(160, 180, 240),
                _ => new Color(220, 160, 220),
            };
        }
    }
}
