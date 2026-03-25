using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheCure
{
    public class TheCure : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        private bool _isEscapeKeyPressed = false;

        public TheCure()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gameManager = GameManager.GetGameManager();
            base.Initialize();

            Player player = new Player(new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

            _gameManager.Initialize(Content, this, player);

            _gameManager.AddGameObject(player);
            _gameManager.AddGameObject(new Alien());
            _gameManager.AddGameObject(new Supply());

            Vector2 asteroidPosition1 = new Vector2(1000, 800);
            Vector2 asteroidPosition2 = new Vector2(-200, 200);
            Vector2 asteroidPosition3 = new Vector2(500, -400);

            Asteroid asteroid1 = new Asteroid(asteroidPosition1);
            Asteroid asteroid2 = new Asteroid(asteroidPosition2);
            Asteroid asteroid3 = new Asteroid(asteroidPosition3);

            _gameManager.AddGameObject(asteroid1);
            _gameManager.AddGameObject(asteroid2);
            _gameManager.AddGameObject(asteroid3);

            asteroid1.Load(Content);
            asteroid2.Load(Content);
            asteroid3.Load(Content);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameManager.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _gameManager.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}