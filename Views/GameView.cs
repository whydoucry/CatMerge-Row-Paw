using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CatMergeRowPaw.Controllers;

namespace CatMergeRowPaw.Views
{
    public class GameView
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _pixel;
        private readonly TextRenderer _textRenderer;

        public Rectangle MatchBoardRect { get; } = new Rectangle(240, 90, 480, 430);
        public Rectangle MergeBoardRect { get; } = new Rectangle(280, 80, 400, 400);
        public Rectangle InventoryPanelRect { get; } = new Rectangle(280, 480, 80, 80);
        public Rectangle MenuCatRect { get; } = new Rectangle(360, 80, 240, 240);
        public Rectangle MatchButtonRect { get; } = new Rectangle(320, 180, 320, 70);
        public Rectangle MergeButtonRect { get; } = new Rectangle(320, 270, 320, 70);
        public Rectangle SettingsButtonRect { get; } = new Rectangle(320, 360, 320, 70);
        public Rectangle BackButtonRect { get; } = new Rectangle(20, 20, 160, 50);
        public Rectangle LevelCompleteRect { get; } = new Rectangle(220, 200, 520, 180);
        public Rectangle LevelCompletePanelRect { get; } = new Rectangle(320, 120, 320, 320);
        public Rectangle LevelNextRect { get; } = new Rectangle(360, 220, 240, 60);
        public Rectangle LevelMenuRect { get; } = new Rectangle(360, 290, 240, 60);
        public Rectangle LevelExitRect { get; } = new Rectangle(360, 360, 240, 60);
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

        public void Draw(GameMode mode, Match3Controller match3, Controllers.MergeController merge, Controllers.GameController controller, Point? selectedMatchCell, Point? selectedMergeCell, bool levelComplete, bool hasCatReward, bool isPaused)
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
                DrawMergeBoard(merge, selectedMergeCell, controller);
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


        private void DrawMergeBoard(Controllers.MergeController merge, Point? selectedCell, Controllers.GameController controller)
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(30, 30, 60));
            DrawRectangle(BackButtonRect, new Color(80, 80, 120));
            DrawBorder(BackButtonRect, Color.White);
            DrawText("Меню", new Vector2(BackButtonRect.X + 14, BackButtonRect.Y + 12), Color.White);
            DrawCenteredTitle("Песочное поле", 10, Color.White);

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

                    var baseColor = GetMergeCellColor(board, x, y);
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

            DrawInventoryPanel(controller);
        }

        private Color GetMergeCellColor(Board board, int x, int y)
        {
            if (board.IsOpenCell[x, y])
            {
                return new Color(120, 80, 40);
            }

            return new Color(225, 205, 170);
        }

        private void DrawInventoryPanel(Controllers.GameController controller)
        {
            DrawRectangle(InventoryPanelRect, new Color(70, 70, 90));
            DrawBorder(InventoryPanelRect, Color.White);
            DrawText("Котики", new Vector2(InventoryPanelRect.X + 8, InventoryPanelRect.Y + 8), Color.White);

            var iconSize = 14;
            var iconSpacing = 18;
            var startX = InventoryPanelRect.X + 8;
            var startY = InventoryPanelRect.Y + 30;
            var drawCount = System.Math.Min(9, controller.CatInventory.Count);

            for (var i = 0; i < drawCount; i++)
            {
                var iconRect = new Rectangle(startX + (i % 3) * iconSpacing, startY + (i / 3) * iconSpacing, iconSize, iconSize);
                DrawRectangle(iconRect, controller.CatInventory[i].GetColor());
                DrawBorder(iconRect, Color.White);
            }

            DrawText(controller.UnplacedCatCount.ToString(), new Vector2(InventoryPanelRect.X + InventoryPanelRect.Width - 24, InventoryPanelRect.Y + InventoryPanelRect.Height - 24), Color.White);
        }

        private void DrawMenu(bool hasCatReward)
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(40, 40, 80));
            DrawCenteredTitle("Cat Merge Row Paw", 100, Color.White);

            DrawRectangle(MatchButtonRect, new Color(80, 80, 120));
            DrawBorder(MatchButtonRect, Color.White);
            DrawCenteredText("Три в ряд", MatchButtonRect, Color.White);

            DrawRectangle(MergeButtonRect, new Color(80, 80, 120));
            DrawBorder(MergeButtonRect, Color.White);
            DrawCenteredText("Песочное поле", MergeButtonRect, Color.White);

            DrawRectangle(SettingsButtonRect, new Color(80, 80, 120));
            DrawBorder(SettingsButtonRect, Color.White);
            DrawCenteredText("Настройки", SettingsButtonRect, Color.White);
        }

        private void DrawLevelCompletePanel()
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(0, 0, 0, 150));
            DrawRectangle(LevelCompletePanelRect, new Color(220, 200, 120));
            DrawBorder(LevelCompletePanelRect, Color.White);

            DrawText("Уровень пройден!", new Vector2(LevelCompletePanelRect.X + 40, LevelCompletePanelRect.Y + 40), Color.Black);

            DrawRectangle(LevelNextRect, new Color(80, 80, 120));
            DrawBorder(LevelNextRect, Color.White);
            DrawText("Следующий уровень", new Vector2(LevelNextRect.X + 30, LevelNextRect.Y + 18), Color.White);

            DrawRectangle(LevelMenuRect, new Color(80, 80, 120));
            DrawBorder(LevelMenuRect, Color.White);
            DrawText("Вернуться в меню", new Vector2(LevelMenuRect.X + 30, LevelMenuRect.Y + 18), Color.White);

            DrawRectangle(LevelExitRect, new Color(80, 80, 120));
            DrawBorder(LevelExitRect, Color.White);
            DrawText("Выйти из игры", new Vector2(LevelExitRect.X + 60, LevelExitRect.Y + 18), Color.White);
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

        private void DrawMatchBoard(Match3Controller match3, Point? selectedCell, bool levelComplete, IEnumerable<Controllers.TileAnimation> animatingTiles)
        {
            DrawRectangle(new Rectangle(0, 0, 960, 560), new Color(20, 20, 50));
            DrawRectangle(BackButtonRect, new Color(80, 80, 120));
            DrawBorder(BackButtonRect, Color.White);
            DrawText("Меню", new Vector2(BackButtonRect.X + 14, BackButtonRect.Y + 12), Color.White);

            var infoPanelWidth = 560;
            var infoPanelHeight = 32;
            var infoPanelX = (960 - infoPanelWidth) / 2;
            var infoPanelY = 10;
            var levelPanel = new Rectangle(infoPanelX, infoPanelY, infoPanelWidth / 2 - 6, infoPanelHeight);
            var remainingPanel = new Rectangle(infoPanelX + infoPanelWidth / 2 + 6, infoPanelY, infoPanelWidth / 2 - 6, infoPanelHeight);

            DrawRectangle(levelPanel, new Color(60, 60, 100));
            DrawBorder(levelPanel, Color.White);
            DrawCenteredText($"Уровень {match3.CurrentLevel}", levelPanel, Color.White);

            DrawRectangle(remainingPanel, new Color(60, 60, 100));
            DrawBorder(remainingPanel, Color.White);
            DrawCenteredText($"Осталось {match3.MatchesRequired - match3.MatchesMade} комбинаций", remainingPanel, Color.White);

            var scoreRect = new Rectangle(infoPanelX, infoPanelY + infoPanelHeight + 10, infoPanelWidth, 24);
            DrawCenteredText($"Счет: {match3.Score}", scoreRect, Color.White);

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

            if (levelComplete && match3.CurrentLevel == 1)
            {
                DrawRectangle(LevelCompleteRect, new Color(0, 0, 0, 180));
                var inner = new Rectangle(LevelCompleteRect.X + 10, LevelCompleteRect.Y + 10, LevelCompleteRect.Width - 20, LevelCompleteRect.Height - 20);
                DrawRectangle(inner, new Color(220, 200, 120));
                DrawBorder(inner, Color.White);
                DrawText("Уровень пройден!", new Vector2(inner.X + 70, inner.Y + 40), Color.Black);
                DrawText("Вернитесь в меню и переходите в Merge.", new Vector2(inner.X + 40, inner.Y + 90), Color.Black);
            }
            else if (levelComplete && match3.CurrentLevel > 1)
            {
                DrawLevelCompletePanel();
            }
        }

        private void DrawText(string text, Vector2 position, Color color)
        {
            var texture = _textRenderer.GetTextTexture(text, color);
            _spriteBatch.Draw(texture, position, Color.White);
        }

        private void DrawCenteredText(string text, Rectangle rect, Color color)
        {
            var texture = _textRenderer.GetTextTexture(text, color);
            var position = new Vector2(rect.X + (rect.Width - texture.Width) / 2, rect.Y + (rect.Height - texture.Height) / 2);
            _spriteBatch.Draw(texture, position, Color.White);
        }

        private void DrawCenteredTitle(string text, int topY, Color color)
        {
            var texture = _textRenderer.GetTextTexture(text, color);
            var position = new Vector2((960 - texture.Width) / 2, topY);
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
