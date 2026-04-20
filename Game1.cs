using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CatMergeRowPaw.Controllers;
using CatMergeRowPaw.Views;

namespace CatMergeRowPaw
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;
        private Texture2D? _pixel;
        private TextRenderer? _textRenderer;
        private GameView? _view;
        private readonly GameController _controller;

        private bool _mousePressedLastFrame;
        private KeyboardState _previousKeyboard;
        private float _scaleX;
        private float _scaleY;
        private bool _stretchToFill = true;
        private int _screenWidth;
        private int _screenHeight;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 960,
                PreferredBackBufferHeight = 560,
                IsFullScreen = true,
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _controller = new GameController();
        }

        protected override void Initialize()
        {
            _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _scaleX = (float)_screenWidth / 960f;
            _scaleY = (float)_screenHeight / 560f;

            _controller.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            _textRenderer = new TextRenderer(GraphicsDevice);
            _view = new GameView(_spriteBatch, _pixel, _textRenderer!);
            _controller.Match3.CellHeight = _view.MatchBoardRect.Height / 8f;
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            if (WasKeyPressed(keyboard, Keys.Escape))
            {
                if (_controller.CurrentMode == GameMode.Match3)
                {
                    _controller.IsPaused = !_controller.IsPaused;
                }
                else
                {
                    Exit();
                }
            }

            if (WasKeyPressed(keyboard, Keys.Tab) || WasKeyPressed(keyboard, Keys.Space))
            {
                _controller.SwitchMode();
            }

            var mouse = Mouse.GetState();
            var mouseClicked = mouse.LeftButton == ButtonState.Pressed && !_mousePressedLastFrame;
            if (mouseClicked && _view != null && !_controller.IsPaused)
            {
                var scaledX = mouse.Position.X / _scaleX;
                var scaledY = mouse.Position.Y / _scaleY;
                var scaledPos = new Point((int)scaledX, (int)scaledY);

                if (_controller.CurrentMode == GameMode.Menu)
                {
                    if (_view.MatchButtonRect.Contains(scaledPos))
                    {
                        _controller.GoToMatch3();
                    }
                    else if (_view.MergeButtonRect.Contains(scaledPos))
                    {
                        _controller.GoToMerge();
                    }
                    else if (_view.SettingsButtonRect.Contains(scaledPos))
                    {
                        _controller.GoToSettings();
                    }
                }
                else if (_controller.CurrentMode == GameMode.Settings)
                {
                    if (_view.SettingsStretchRect.Contains(scaledPos))
                    {
                        _controller.StretchToFill = !_controller.StretchToFill;
                    }
                    else if (_view.BackButtonRect.Contains(scaledPos))
                    {
                        _controller.GoToMenu();
                    }
                }
            }

            // Handle pause menu clicks
            if (_controller.IsPaused && mouseClicked && _view != null)
            {
                var scaledX = mouse.Position.X / _scaleX;
                var scaledY = mouse.Position.Y / _scaleY;
                var scaledPos = new Point((int)scaledX, (int)scaledY);

                if (_view.PauseContinueRect.Contains(scaledPos))
                {
                    _controller.IsPaused = false;
                }
                else if (_view.PauseMenuRect.Contains(scaledPos))
                {
                    _controller.GoToMenu();
                    _controller.IsPaused = false;
                }
                else if (_view.PauseExitRect.Contains(scaledPos))
                {
                    Exit();
                }
            }

            if (!_controller.IsPaused)
            {
                _controller.Update(gameTime);
            }
            Window.Title = $"Cat Merge Row Paw - Mode: {_controller.CurrentMode} - Score: {_controller.Match3.Score} - Matches: {_controller.Match3.MatchesMade} - Level: {_controller.Match3.CurrentLevel}";
            _mousePressedLastFrame = mouse.LeftButton == ButtonState.Pressed;
            _previousKeyboard = keyboard;
            base.Update(gameTime);
        }

        private bool WasKeyPressed(KeyboardState keyboard, Keys key)
        {
            return keyboard.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key);
        }

        private static Point? GetCellAtPoint(Point point, Rectangle boardArea, int width, int height)
        {
            if (!boardArea.Contains(point))
            {
                return null;
            }

            var cellWidth = boardArea.Width / width;
            var cellHeight = boardArea.Height / height;
            var x = (point.X - boardArea.X) / cellWidth;
            var y = (point.Y - boardArea.Y) / cellHeight;
            return new Point(x, y);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (_spriteBatch == null || _pixel == null || _view == null || _textRenderer == null)
            {
                return;
            }

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(_scaleX, _scaleY));
            _view.Draw(_controller.CurrentMode, _controller.Match3, _controller.Merge, _controller, _controller.SelectedMatchCell, _controller.SelectedMergeCell, _controller.Match3.IsLevelComplete(), _controller.HasCatReward, _controller.IsPaused);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public enum GameMode
    {
        Menu,
        Match3,
        Merge,
        Settings
    }
}
