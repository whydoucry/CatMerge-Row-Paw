using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatMergeRowPaw
{
    public class Match3System
    {
        private readonly Random _random = new();
        public int Score { get; private set; }
        public int MatchesMade { get; private set; }
        public int CurrentLevel { get; private set; } = 1;
        public int MatchesRequired { get; private set; } = 5;

        public void StartNewGame(Board board)
        {
            Score = 0;
            StartLevel(board, 1);
        }

        public void StartLevel(Board board, int level)
        {
            CurrentLevel = level;
            MatchesRequired = 5 + (level - 1) * 2;
            MatchesMade = 0;
            InitializeBoard(board, false);
        }

        private void InitializeBoard(Board board, bool resetScore)
        {
            if (resetScore)
            {
                Score = 0;
            }

            do
            {
                for (var y = 0; y < board.Height; y++)
                {
                    for (var x = 0; x < board.Width; x++)
                    {
                        board.Tiles[x, y] = CreateRandomTile();
                    }
                }

                for (var y = 0; y < board.Height; y++)
                {
                    for (var x = 0; x < board.Width; x++)
                    {
                        board.Tiles[x, y]!.IsSand = _random.NextDouble() < 0.8;
                    }
                }
            }
            while (HasAnyMatch(board));
        }

        public void PrepareNextLevel(Board board)
        {
            StartLevel(board, CurrentLevel + 1);
        }

        public bool IsLevelComplete()
        {
            return MatchesMade >= MatchesRequired;
        }

        public bool TrySwap(Board board, Point source, Point target)
        {
            if (IsLevelComplete())
            {
                return false;
            }

            if (!board.IsInside(source.X, source.Y) || !board.IsInside(target.X, target.Y))
            {
                return false;
            }

            if (board.Tiles[source.X, source.Y] == null || board.Tiles[target.X, target.Y] == null)
            {
                return false;
            }

            if (Math.Abs(source.X - target.X) + Math.Abs(source.Y - target.Y) != 1)
            {
                return false;
            }

            SwapTiles(board, source, target);
            if (HasAnyMatch(board))
            {
                return true;
            }

            // Swap back if no match was created.
            SwapTiles(board, source, target);
            return false;
        }

        public HashSet<Point> ResolveMatches(Board board)
        {
            var allMatches = new HashSet<Point>();
            var matches = FindMatches(board);
            if (!matches.Any())
            {
                return allMatches;
            }

            MatchesMade++;
            Score += matches.Count * 10;
            ClearSandAroundMatches(board, matches);
            foreach (var point in matches)
            {
                allMatches.Add(point);
                board.Tiles[point.X, point.Y] = null;
            }

            // Resolve chained matches automatically.
            var chained = ResolveMatches(board);
            foreach (var point in chained)
            {
                allMatches.Add(point);
            }

            return allMatches;
        }

        public Tile CreateRandomTile()
        {
            var type = (TileType)_random.Next(Enum.GetValues(typeof(TileType)).Length);
            return new Tile(type, false);
        }

        private void SwapTiles(Board board, Point a, Point b)
        {
            var temp = board.Tiles[a.X, a.Y];
            board.Tiles[a.X, a.Y] = board.Tiles[b.X, b.Y];
            board.Tiles[b.X, b.Y] = temp;
        }

        private HashSet<Point> FindMatches(Board board)
        {
            var matched = new HashSet<Point>();

            for (var y = 0; y < board.Height; y++)
            {
                var runStart = 0;
                for (var x = 1; x <= board.Width; x++)
                {
                    if (x < board.Width
                        && board.Tiles[x, y] != null
                        && board.Tiles[runStart, y] != null
                        && board.Tiles[x, y]!.Type == board.Tiles[runStart, y]!.Type)
                    {
                        continue;
                    }

                    if (board.Tiles[runStart, y] != null)
                    {
                        var runLength = x - runStart;
                        if (runLength >= 3)
                        {
                            for (var i = runStart; i < x; i++)
                            {
                                matched.Add(new Point(i, y));
                            }
                        }
                    }

                    runStart = x;
                }
            }

            for (var x = 0; x < board.Width; x++)
            {
                var runStart = 0;
                for (var y = 1; y <= board.Height; y++)
                {
                    if (y < board.Height
                        && board.Tiles[x, y] != null
                        && board.Tiles[x, runStart] != null
                        && board.Tiles[x, y]!.Type == board.Tiles[x, runStart]!.Type)
                    {
                        continue;
                    }

                    if (board.Tiles[x, runStart] != null)
                    {
                        var runLength = y - runStart;
                        if (runLength >= 3)
                        {
                            for (var i = runStart; i < y; i++)
                            {
                                matched.Add(new Point(x, i));
                            }
                        }
                    }

                    runStart = y;
                }
            }

            return matched;
        }

        private void ClearMatchedTiles(Board board, HashSet<Point> matched)
        {
            foreach (var point in matched)
            {
                board.Tiles[point.X, point.Y] = null;
            }

            ApplyGravity(board);
            FillEmptyTiles(board);
        }

        private void ApplyGravity(Board board)
        {
            for (var x = 0; x < board.Width; x++)
            {
                var writeY = board.Height - 1;
                for (var y = board.Height - 1; y >= 0; y--)
                {
                    if (board.Tiles[x, y] != null)
                    {
                        board.Tiles[x, writeY] = board.Tiles[x, y];
                        if (writeY != y)
                        {
                            board.Tiles[x, y] = null;
                        }
                        writeY--;
                    }
                }
            }
        }

        private void FillEmptyTiles(Board board)
        {
            for (var x = 0; x < board.Width; x++)
            {
                for (var y = 0; y < board.Height; y++)
                {
                    if (board.Tiles[x, y] == null)
                    {
                        board.Tiles[x, y] = CreateRandomTile();
                        board.Tiles[x, y]!.IsSand = false;
                    }
                }
            }
        }

        private void ClearSandAroundMatches(Board board, HashSet<Point> matched)
        {
            foreach (var point in matched)
            {
                var tile = board.Tiles[point.X, point.Y];
                if (tile != null)
                {
                    tile.IsSand = false;
                }

                foreach (var offset in new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
                {
                    var neighbor = new Point(point.X + offset.X, point.Y + offset.Y);
                    if (board.IsInside(neighbor.X, neighbor.Y) && board.Tiles[neighbor.X, neighbor.Y] != null)
                    {
                        board.Tiles[neighbor.X, neighbor.Y]!.IsSand = false;
                    }
                }
            }
        }

        public bool HasAnyMatch(Board board)
        {
            return FindMatches(board).Count > 0;
        }
    }
}
