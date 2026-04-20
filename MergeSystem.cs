using System;
using Microsoft.Xna.Framework;

namespace CatMergeRowPaw
{
    public class MergeSystem
    {
        private readonly Random _random = new();

        public void InitializeMergeBoard(Board board)
        {
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    board.Cats[x, y] = null;
                    board.IsOpenCell[x, y] = (x + y) % 2 == 0;
                }
            }
        }

        public bool AddCat(Board board, Cat cat)
        {
            var emptySlots = new System.Collections.Generic.List<Point>();
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    if (board.Cats[x, y] == null)
                    {
                        emptySlots.Add(new Point(x, y));
                    }
                }
            }

            if (emptySlots.Count == 0)
            {
                return false;
            }

            var slot = emptySlots[_random.Next(emptySlots.Count)];
            board.Cats[slot.X, slot.Y] = cat;
            return true;
        }

        public bool CanMerge(Board board, Point source, Point target)
        {
            if (!board.IsInside(source.X, source.Y) || !board.IsInside(target.X, target.Y))
            {
                return false;
            }

            var sourceCat = board.Cats[source.X, source.Y];
            var targetCat = board.Cats[target.X, target.Y];
            if (sourceCat == null || targetCat == null)
            {
                return false;
            }

            return sourceCat.Level == targetCat.Level;
        }

        public bool TryMerge(Board board, Point source, Point target)
        {
            if (!CanMerge(board, source, target))
            {
                return false;
            }

            var targetCat = board.Cats[target.X, target.Y]!;
            board.Cats[source.X, source.Y] = null;
            board.Cats[target.X, target.Y] = targetCat.Upgrade();
            return true;
        }
    }
}
