using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CatMergeRowPaw.Views
{
    public class GameView
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _pixel;
        private readonly TextRenderer _textRenderer;

        public Rectangle MatchBoardRect { get; } = new Rectangle(240, 40, 480, 480);
        public Rectangle MergeBoardRect { get; } = new Rectangle(280, 80, 400, 400);
        public Rectangle MenuCatRect { get; } = new Rectangle(360, 80, 240, 240);
        public Rectangle MatchButtonRect { get; } = new Rectangle(320, 300, 320, 70);
        public Rectangle MergeButtonRect { get; } = new Rectangle(320, 380, 320, 70);
        public Rectangle SettingsButtonRect { get; } = new Rectangle(320, 460, 320, 70);
        public Rectangle BackButtonRect { get; } = new Rectangle(20, 20, 160, 50);
        public Rectangle LevelCompleteRect { get; } = new Rectangle(220, 200, 520, 180);
        public Rectangle PauseContinueRect { get; } = new Rectangle(380, 200, 200, 50);
        public Rectangle PauseMenuRect { get; } = new Rectangle(380, 270, 200, 50);
        public Rectangle PauseExitRect { get; } = new Rectangle(380, 340, 200, 50);
        public Rectangle SettingsStretchRect { get; } = new Rectangle(320, 300, 320, 70);

        public GameView(SpriteBatch spriteBatch, Texture2D pixel, TextRenderer textRenderer)
        {
            _spriteBatch = spriteBatch;
            _pixel = pixel;
            _textRenderer = textRenderer;
        }

        public void Draw(GameMode mode, Controllers.Match3Controller match3, Controllers.MergeController merge, Controllers.GameController controller, Point? selectedMatchCell, Point? selectedMergeCell, bool levelComplete, bool hasCatReward, bool isPaused)
        {
            if (mode == GameMode.Menu)
            {
                DrawMenu(hasCatReward);
            }
            else if (mode == GameMode.Match3)
            {
                DrawMatchBoard(match3, selectedMatchCell, levelComplete, match3.GetAnimatingTiles());
            }
            else if (mode == GameMode.Merge)
            {
                DrawMergeBoard(merge, selectedMergeCell);
            }
            else if (mode == GameMode.Settings)
            {
                DrawSettings(controller);
            }

            if (isPaused)
            {
                DrawPauseMenu();
            }
        }


        private void DrawMergeBoard(Controllers.MergeController merge, Point? selectedCell)
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(30, 30, 60));
            DrawRectangle(BackButtonRect, new Color(80, 80, 120));
            DrawBorder(BackButtonRect, Color.White);
            DrawText("Меню", new Vector2(BackButtonRect.X + 14, BackButtonRect.Y + 12), Color.White);
            DrawText("Merge поле", new Vector2(520, 10), Color.White);

            DrawRectangle(MergeBoardRect, Color.Black);
            var board = merge.Board;
            var cellWidth = MergeBoardRect.Width / board.Width;
            var cellHeight = MergeBoardRect.Height / board.Height;

            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    var cellRect = new Rectangle(
                        MergeBoardRect.X + x * cellWidth,
                        MergeBoardRect.Y + y * cellHeight,
                        cellWidth - 4,
                        cellHeight - 4);

                    var baseColor = board.IsOpenCell[x, y] ? new Color(180, 220, 180) : new Color(120, 120, 120);
                    DrawRectangle(cellRect, baseColor);

                    if (board.Cats[x, y] != null)
                    {
                        var cat = board.Cats[x, y]!;
                        var inner = new Rectangle(cellRect.X + 6, cellRect.Y + 6, cellRect.Width - 12, cellRect.Height - 12);
                        DrawRectangle(inner, cat.GetColor());
                        if (cat.Level > 1)
                        {
                            var levelRect = new Rectangle(inner.X + 4, inner.Y + 4, 10, 10);
                            DrawRectangle(levelRect, Color.White);
                        }
                    }

                    if (selectedCell.HasValue && selectedCell.Value.X == x && selectedCell.Value.Y == y)
                    {
                        DrawBorder(cellRect, Color.Yellow);
                    }
                }
            }
        }

        private void DrawMenu(bool hasCatReward)
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(40, 40, 80));
            DrawText("Cat Merge Row Paw", new Vector2(300, 20), Color.White);
            DrawRectangle(MenuCatRect, new Color(220, 180, 180));
            DrawBorder(MenuCatRect, Color.White);
            DrawText("Ваш кот", new Vector2(MenuCatRect.X + 40, MenuCatRect.Y + MenuCatRect.Height + 10), Color.White);

            DrawRectangle(MatchButtonRect, new Color(80, 80, 120));
            DrawBorder(MatchButtonRect, Color.White);
            DrawText("Три в ряд", new Vector2(MatchButtonRect.X + 90, MatchButtonRect.Y + 20), Color.White);

            DrawRectangle(MergeButtonRect, new Color(80, 80, 120));
            DrawBorder(MergeButtonRect, Color.White);
            DrawText("Песочное поле", new Vector2(MergeButtonRect.X + 60, MergeButtonRect.Y + 20), Color.White);

            DrawRectangle(SettingsButtonRect, new Color(80, 80, 120));
            DrawBorder(SettingsButtonRect, Color.White);
            DrawText("Настройки", new Vector2(SettingsButtonRect.X + 90, SettingsButtonRect.Y + 20), Color.White);

            if (hasCatReward)
            {
                var badge = new Rectangle(MenuCatRect.X + MenuCatRect.Width - 60, MenuCatRect.Y + 10, 50, 50);
                DrawRectangle(badge, new Color(220, 240, 120));
                DrawBorder(badge, Color.White);
                DrawText("!", new Vector2(badge.X + 18, badge.Y + 8), Color.Black);
            }
        }

        private void DrawSettings(Controllers.GameController controller)
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(40, 40, 80));
            DrawText("Настройки", new Vector2(400, 20), Color.White);

            DrawRectangle(SettingsStretchRect, new Color(80, 80, 120));
            DrawBorder(SettingsStretchRect, Color.White);
            DrawText("Растянуть на экран: " + (controller.StretchToFill ? "Вкл" : "Выкл"), new Vector2(SettingsStretchRect.X + 20, SettingsStretchRect.Y + 20), Color.White);

            DrawRectangle(BackButtonRect, new Color(80, 80, 120));
            DrawBorder(BackButtonRect, Color.White);
            DrawText("Назад", new Vector2(BackButtonRect.X + 14, BackButtonRect.Y + 12), Color.White);
        }

        private void DrawMatchBoard(Controllers.Match3Controller match3, Point? selectedCell, bool levelComplete, IEnumerable<Controllers.TileAnimation> animatingTiles)
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(20, 20, 50));
            DrawRectangle(BackButtonRect, new Color(80, 80, 120));
            DrawBorder(BackButtonRect, Color.White);
            DrawText("Меню", new Vector2(BackButtonRect.X + 14, BackButtonRect.Y + 12), Color.White);
            DrawText($"Уровень {match3.CurrentLevel}", new Vector2(520, 10), Color.White);
            DrawText($"Матчи: {match3.MatchesMade}/{match3.MatchesRequired}", new Vector2(520, 40), Color.White);
            DrawText($"Счет: {match3.Score}", new Vector2(520, 70), Color.White);

            var board = match3.Board;
            DrawRectangle(MatchBoardRect, Color.Black);
            var cellWidth = MatchBoardRect.Width / board.Width;
            var cellHeight = MatchBoardRect.Height / board.Height;

            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    var tile = board.Tiles[x, y];
                    var cellRect = new Rectangle(
                        MatchBoardRect.X + x * cellWidth,
                        MatchBoardRect.Y + y * cellHeight,
                        cellWidth - 2,
                        cellHeight - 2);

                    var yOffset = match3.GetTileYOffset(new Point(x, y));
                    cellRect.Y += (int)yOffset;

                    if (tile != null)
                    {
                        DrawRectangle(cellRect, tile.GetColor());
                        if (tile.IsSand)
                        {
                            var sandColor = new Color(210, 180, 140, 180);
                            DrawRectangle(cellRect, sandColor);
                        }
                    }
                    else
                    {
                        DrawRectangle(cellRect, new Color(50, 50, 80));
                    }

                    if (selectedCell.HasValue && selectedCell.Value.X == x && selectedCell.Value.Y == y)
                    {
                        DrawBorder(cellRect, Color.White);
                    }
                }
            }

            // Draw animating tiles
            foreach (var anim in animatingTiles)
            {
                if (anim.Tile != null)
                {
                    var color = anim.Tile.Color;
                    var tileColor = GetTileColor(color);
                    var rect = new Rectangle((int)(anim.Position.X - cellWidth / 2), (int)(anim.Position.Y - cellHeight / 2), (int)cellWidth - 4, (int)cellHeight - 4);
                    DrawRectangle(rect, tileColor);
                    DrawBorder(rect, Color.White);
                    if (anim.Tile.IsSand)
                    {
                        var sandColor = new Color(210, 180, 140, 180);
                        DrawRectangle(rect, sandColor);
                    }
                }
            }

            if (levelComplete && match3.CurrentLevel == 1)
            {
                DrawRectangle(LevelCompleteRect, new Color(0, 0, 0, 180));
                var inner = new Rectangle(LevelCompleteRect.X + 10, LevelCompleteRect.Y + 10, LevelCompleteRect.Width - 20, LevelCompleteRect.Height - 20);
                DrawRectangle(inner, new Color(220, 200, 120));
                DrawBorder(inner, Color.White);
                DrawText("Уровень пройден!", new Vector2(inner.X + 70, inner.Y + 40), Color.Black);
                DrawText("Вернитесь в меню и переходите в Merge.", new Vector2(inner.X + 40, inner.Y + 90), Color.Black);
            }
        }

        private void DrawText(string text, Vector2 position, Color color)
        {
            var texture = _textRenderer.GetTextTexture(text, color);
            _spriteBatch.Draw(texture, position, Color.White);
        }

        private void DrawRectangle(Rectangle rect, Color color)
        {
            _spriteBatch.Draw(_pixel, rect, color);
        }

        private void DrawPauseMenu()
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(0, 0, 0, 180));
            DrawRectangle(PauseContinueRect, Color.LightBlue);
            DrawBorder(PauseContinueRect, Color.White);
            DrawText("Продолжить игру", new Vector2(PauseContinueRect.X + 10, PauseContinueRect.Y + 10), Color.Black);

            DrawRectangle(PauseMenuRect, Color.LightGreen);
            DrawBorder(PauseMenuRect, Color.White);
            DrawText("Вернуться в меню", new Vector2(PauseMenuRect.X + 10, PauseMenuRect.Y + 10), Color.Black);

            DrawRectangle(PauseExitRect, Color.LightCoral);
            DrawBorder(PauseExitRect, Color.White);
            DrawText("Выйти из игры", new Vector2(PauseExitRect.X + 10, PauseExitRect.Y + 10), Color.Black);
        }

        private void DrawBorder(Rectangle rect, Color color)
        {
            DrawRectangle(new Rectangle(rect.X, rect.Y, rect.Width, 2), color);
            DrawRectangle(new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), color);
            DrawRectangle(new Rectangle(rect.X, rect.Y, 2, rect.Height), color);
            DrawRectangle(new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), color);
        }
    }
}
