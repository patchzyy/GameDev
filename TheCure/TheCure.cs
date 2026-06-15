using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Engine.Managers;
using TheCure.Managers;

namespace TheCure
{
    public class TheCure : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        private PlayerManager _playerManager;
        private ContentsManager _contentsManager;
        private ScreenManager _screenManager;
        private InputManager _inputManager;
        private UpgradeManager _upgradeManager;
        private PassivesManager _passivesManager;
        private BoostManager _boostManager;
        private SoundManager _soundManager;
        private bool _isEscapeKeyPressed = false;

        public TheCure()
        {
            Settings.Load();
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Settings.GetValue(SettingsConst.VIDEO.WIDTH);
            _graphics.PreferredBackBufferHeight = Settings.GetValue(SettingsConst.VIDEO.HEIGHT);
            _graphics.IsFullScreen = Settings.GetValue(SettingsConst.VIDEO.DISPLAY_MODE) == DisplayModeSetting.Fullscreen;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gameManager = GameManager.Get();
            _contentsManager = ContentsManager.Get();
            _playerManager = PlayerManager.Get();
            _screenManager = ScreenManager.Get();
            _inputManager = InputManager.Get();
            _upgradeManager = UpgradeManager.Get();
            _passivesManager = PassivesManager.Get();
            _boostManager = BoostManager.Get();
            _soundManager = SoundManager.Get();
            _graphics.ApplyChanges();

            _contentsManager.Initialize(Content, this);

            Player player =
                new Player(new Vector2(
                    GraphicsDevice.Viewport.Width / 2,
                    GraphicsDevice.Viewport.Height / 2
                ));

            _playerManager.Initialize(player);
            _gameManager.Initialize(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _contentsManager.Load();
            _gameManager.Load();
            _screenManager.Load();
            _inputManager.Load();
            _upgradeManager.Load();
            _passivesManager.Load();
            _boostManager.Load();
            _soundManager.Load();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && !_isEscapeKeyPressed)
            {
                if (_gameManager.CurrentGameState == GameState.Playing ||
                    _gameManager.CurrentGameState == GameState.Upgrade ||
                    _gameManager.CurrentGameState == GameState.PassiveUpgrade)
                    _gameManager.PauseGame();
                else if (_gameManager.CurrentGameState == GameState.Paused)
                    _gameManager.ResumeGame();
                else if (_gameManager.CurrentGameState == GameState.StartScreen)
                    Exit();
            }

            _isEscapeKeyPressed = currentKeyboardState.IsKeyDown(Keys.Escape);
            _gameManager.Update(gameTime);
            _screenManager.Update();
            _inputManager.Update();

            base.Update(gameTime);
        }

        public void ApplyVideoSettings(int width, int height, DisplayModeSetting mode)
        {
            _graphics.IsFullScreen = false;
            Window.IsBorderless = false;

            _graphics.ApplyChanges();

            switch (mode)
            {
                case DisplayModeSetting.Windowed:
                    _graphics.PreferredBackBufferWidth = width;
                    _graphics.PreferredBackBufferHeight = height;

                    break;

                case DisplayModeSetting.Borderless:
                    var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                    Window.IsBorderless = true;

                    _graphics.PreferredBackBufferWidth = displayMode.Width;
                    _graphics.PreferredBackBufferHeight = displayMode.Height;

                    break;

                case DisplayModeSetting.Fullscreen:
                    _graphics.PreferredBackBufferWidth = width;
                    _graphics.PreferredBackBufferHeight = height;

                    _graphics.IsFullScreen = true;
                    break;
            }

            _graphics.ApplyChanges();

            Settings.Save(SettingsConst.VIDEO.WIDTH, width);
            Settings.Save(SettingsConst.VIDEO.HEIGHT, height);
            Settings.Save(SettingsConst.VIDEO.DISPLAY_MODE, mode);
        }

        protected override void Draw(GameTime gameTime)
        {
            _gameManager.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}