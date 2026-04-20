using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CatMergeRowPaw.Controllers
{
    public class TileAnimation
    {
        public Vector2 Position { get; set; }
        public float Offset { get; set; }
        public float Progress { get; set; }
        public Tile? Tile { get; set; }
    }

    public class Match3Controller
    {
        public Board Board { get; }
        private readonly Match3System _match3System;
        public float CellHeight { get; set; } = 60f; // Default, will be set from view

        private List<TileAnimation> _animatingTiles = new();
        private const float FallSpeed = 300f; // pixels per second

        public int Score => _match3System.Score;
        public int MatchesMade => _match3System.MatchesMade;
        public int CurrentLevel => _match3System.CurrentLevel;
        public IEnumerable<TileAnimation> GetAnimatingTiles() => _animatingTiles;

        public Match3Controller()
        {
            Board = new Board(8, 8);
            _match3System = new Match3System();
        }

        public void Initialize()
        {
            _match3System.StartNewGame(Board);
        }

        public bool TrySwap(Point source, Point target)
        {
            if (_animatingTiles.Count > 0) return false;
            if (_match3System.TrySwap(Board, source, target))
            {
                ResolveMatches();
                return true;
            }
            return false;
        }

        public bool IsLevelComplete()
        {
            return _match3System.IsLevelComplete();
        }

        public void PrepareNextLevel()
        {
            _match3System.PrepareNextLevel(Board);
        }

        public void ResolveMatches()
        {
            var matched = _match3System.ResolveMatches(Board);
            if (matched.Any())
            {
                StartFallAnimation();
            }
        }

        private void StartFallAnimation()
        {
            _animatingTiles.Clear();
            for (int x = 0; x < Board.Width; x++)
            {
                int writeY = Board.Height - 1;
                for (int readY = Board.Height - 1; readY >= 0; readY--)
                {
                    if (Board.Tiles[x, readY] != null)
                    {
                        if (readY != writeY)
                        {
                            // Animate falling
                            _animatingTiles.Add(new TileAnimation
                            {
                                Position = new Vector2(x * CellHeight + CellHeight / 2, readY * CellHeight + CellHeight / 2),
                                Offset = (writeY - readY) * CellHeight,
                                Tile = Board.Tiles[x, readY]
                            });
                            Board.Tiles[x, writeY] = Board.Tiles[x, readY];
                            Board.Tiles[x, readY] = null;
                        }
                        writeY--;
                    }
                }
                // Fill top with new tiles
                for (int y = 0; y <= writeY; y++)
                {
                    Board.Tiles[x, y] = _match3System.CreateRandomTile();
                    Board.Tiles[x, y]!.IsSand = false;
                    _animatingTiles.Add(new TileAnimation
                    {
                        Position = new Vector2(x * CellHeight + CellHeight / 2, y * CellHeight + CellHeight / 2 - (Board.Height - y) * CellHeight),
                        Offset = (Board.Height - y) * CellHeight,
                        Tile = Board.Tiles[x, y]
                    });
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _animatingTiles.Count - 1; i >= 0; i--)
            {
                var anim = _animatingTiles[i];
                anim.Position.Y += anim.Offset * (float)gameTime.ElapsedGameTime.TotalSeconds;
                anim.Progress += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _animatingTiles[i] = anim;
                if (anim.Progress >= 1)
                {
                    _animatingTiles.RemoveAt(i);
                }
            }

            if (_animatingTiles.Count == 0 && HasAnyMatch())
            {
                ResolveMatches();
            }
        }

        private bool HasAnyMatch()
        {
            return _match3System.HasAnyMatch(Board);
        }

        public float GetTileYOffset(Point position)
        {
            var anim = _animatingTiles.Find(a => a.position == position);
            return anim.yOffset;
        }

        public bool IsAnimating => _animatingTiles.Count > 0;
    }
}
