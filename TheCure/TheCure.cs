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
        private BoostManager _boostManager;
        private bool _isEscapeKeyPressed = false;

        public TheCure()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

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
            _boostManager = BoostManager.Get();

            _contentsManager.Initialize(Content, this);

            Player player =
                new Player(new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

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
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && !_isEscapeKeyPressed)
            {
                if (_gameManager.CurrentGameState == GameState.Playing)
                {
                    _gameManager.SetGameState(GameState.Paused);
                }
                else if (_gameManager.CurrentGameState == GameState.Paused)
                {
                    _gameManager.SetGameState(GameState.Playing);
                }
                else if (_gameManager.CurrentGameState == GameState.StartScreen)
                {
                    Exit();
                }
            }

            _isEscapeKeyPressed = currentKeyboardState.IsKeyDown(Keys.Escape);
            _gameManager.Update(gameTime);
            _screenManager.Update();
            _inputManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _gameManager.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}