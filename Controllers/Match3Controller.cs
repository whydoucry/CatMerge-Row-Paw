using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CatMergeRowPaw.Controllers
{
    public class TileAnimation
    {
        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }
        public Vector2 Position { get; set; }
        public float Progress { get; set; }
        public Tile? Tile { get; set; }
        public Point GridPosition { get; set; }
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
        public int MatchesRequired => _match3System.MatchesRequired;
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
            if (IsLevelComplete())
            {
                return;
            }

            var matched = _match3System.ResolveMatches(Board);
            if (matched.Any())
            {
                StartFallAnimation();
            }
        }

        private void StartFallAnimation()
        {
            _animatingTiles.Clear();
            var halfCell = CellHeight / 2f;

            for (int x = 0; x < Board.Width; x++)
            {
                int writeY = Board.Height - 1;
                for (int readY = Board.Height - 1; readY >= 0; readY--)
                {
                    if (Board.Tiles[x, readY] != null)
                    {
                        if (readY != writeY)
                        {
                            var startPosition = new Vector2(x * CellHeight + halfCell, readY * CellHeight + halfCell);
                            var endPosition = new Vector2(x * CellHeight + halfCell, writeY * CellHeight + halfCell);
                            _animatingTiles.Add(new TileAnimation
                            {
                                StartPosition = startPosition,
                                EndPosition = endPosition,
                                Position = startPosition,
                                Progress = 0f,
                                Tile = Board.Tiles[x, readY],
                                GridPosition = new Point(x, writeY)
                            });
                            Board.Tiles[x, writeY] = Board.Tiles[x, readY];
                            Board.Tiles[x, readY] = null;
                        }
                        writeY--;
                    }
                }

                for (int y = 0; y <= writeY; y++)
                {
                    Board.Tiles[x, y] = _match3System.CreateRandomTile();
                    Board.Tiles[x, y]!.IsSand = false;
                    var endPosition = new Vector2(x * CellHeight + halfCell, y * CellHeight + halfCell);
                    var startPosition = new Vector2(x * CellHeight + halfCell, endPosition.Y - Board.Height * CellHeight);
                    _animatingTiles.Add(new TileAnimation
                    {
                        StartPosition = startPosition,
                        EndPosition = endPosition,
                        Position = startPosition,
                        Progress = 0f,
                        Tile = Board.Tiles[x, y],
                        GridPosition = new Point(x, y)
                    });
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _animatingTiles.Count - 1; i >= 0; i--)
            {
                var anim = _animatingTiles[i];
                var distance = Vector2.Distance(anim.StartPosition, anim.EndPosition);
                if (distance <= 0f)
                {
                    anim.Progress = 1f;
                    anim.Position = anim.EndPosition;
                }
                else
                {
                    var delta = FallSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    anim.Progress = Math.Min(1f, anim.Progress + delta / distance);
                    anim.Position = Vector2.Lerp(anim.StartPosition, anim.EndPosition, anim.Progress);
                }

                _animatingTiles[i] = anim;
                if (anim.Progress >= 1f)
                {
                    _animatingTiles.RemoveAt(i);
                }
            }

            if (_animatingTiles.Count == 0 && !IsLevelComplete() && HasAnyMatch())
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
            var anim = _animatingTiles.Find(a => a.GridPosition == position);
            if (anim == null)
            {
                return 0f;
            }

            var targetY = position.Y * CellHeight + CellHeight / 2f;
            return anim.Position.Y - targetY;
        }

        public bool IsAnimating => _animatingTiles.Count > 0;
    }
}
