using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheCure
{
    public class GameManager
    {
        private static GameManager gameManager;

        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        private ContentManager _content;
        private Texture2D _backgroundTexture;
        private SpriteFont _titleFont;
        private SpriteFont _buttonFont;
        private Button _startButton;
        private Button _quitButton;
        private Button _continueButton;
        private Button _pauseQuitButton;
        private Button _restartButton;
        private Camera _camera;
        private HUD _hud;
        private int _score = 0;
        private Texture2D _buttonTexture;

        private float _gameTimeElapsed = 0f;
        private float _spawnTimer = 0f;

        private float _initialSpawnInterval = 5.0f;
        private float _currentSpawnInterval;

        private int _enemiesToSpawn = 1;
        private int _maxEnemiesOnScreen;

        private float _supplySpawnTimer = 0f;
        private float _supplySpawnInterval = 15.0f;


        public Random RNG { get; private set; }
        public Player Player { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }
        public Texture2D DummyTexture { get; private set; }
        public GameState CurrentGameState { get; private set; }
        public List<Zombie> Zombies;


        public List<Friendly> Friendlies { get; private set; } = new List<Friendly>();

        public static GameManager GetGameManager()
        {
            if (gameManager == null)
            {
                gameManager = new GameManager();
            }

            return gameManager;
        }

        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            Zombies = new List<Zombie>();

            InputManager = new InputManager();
            RNG = new Random();

            CurrentGameState = GameState.StartScreen;
            _currentSpawnInterval = _initialSpawnInterval;
        }

        public void Initialize(ContentManager content, Game game, Player player)
        {
            Game = game;
            _content = content;
            Player = player;
            _camera = new Camera(Game.GraphicsDevice.Viewport);

            DummyTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            DummyTexture.SetData(new Color[] { Color.White });

            int buttonWidth = 200;
            int buttonHeight = 50;
            int centerX = Game.GraphicsDevice.Viewport.Width / 2;
            int centerY = Game.GraphicsDevice.Viewport.Height / 2;
            int spacing = 20;

            _startButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, centerY - buttonHeight - spacing / 2, buttonWidth,
                    buttonHeight),
                "Start",
                _buttonFont);
            _startButton.Clicked += StartButton_Clicked;

            _quitButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, centerY + spacing / 2, buttonWidth, buttonHeight),
                "Quit",
                _buttonFont);
            _quitButton.Clicked += QuitButton_Clicked;

            _continueButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, (int)(centerY - buttonHeight - spacing * 1.5f), buttonWidth,
                    buttonHeight),
                "Continue",
                _buttonFont);
            _continueButton.Clicked += ContinueButton_Clicked;

            _pauseQuitButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, (int)(centerY + spacing * 3.5f), buttonWidth, buttonHeight),
                "Quit",
                _buttonFont);
            _pauseQuitButton.Clicked += PauseQuitButton_Clicked;

            _restartButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, (int)(centerY - spacing * 4f), buttonWidth, buttonHeight),
                "Opnieuw spelen",
                _buttonFont);
            _restartButton.Clicked += RestartButton_Clicked;

            Vector2 pickupPos = new Vector2(-500, -300);
            Vector2 dropOffPos = new Vector2(2000, 600);

            CurrentGameState = GameState.StartScreen;
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            CurrentGameState = GameState.Playing;
        }

        private void ContinueButton_Clicked(object sender, EventArgs e)
        {
            CurrentGameState = GameState.Playing;
        }

        private void QuitButton_Clicked(object sender, EventArgs e)
        {
            Game.Exit();
        }

        private void PauseQuitButton_Clicked(object sender, EventArgs e)
        {
            Game.Exit();
        }

        private void RestartButton_Clicked(object sender, EventArgs e)
        {
            ResetGame();
            CurrentGameState = GameState.Playing;
        }

        private void ResetGame()
        {
            _gameObjects.Clear();
            _toBeRemoved.Clear();
            _toBeAdded.Clear();

            _score = 0;

            Player.GainHealth((int)Player.MaxHealth);
            Player._currentWeapon = Player._bulletWeapon;
            Player._weaponBuffTimer = 0f;
            Player._rectangleCollider.shape.Location = new Point(Game.GraphicsDevice.Viewport.Width / 2,
                Game.GraphicsDevice.Viewport.Height / 2);
            Player._velocity = Vector2.Zero;
            Player._rotation = 0f;

            _gameTimeElapsed = 0f;
            _spawnTimer = 0f;
            _supplySpawnTimer = 0f;
            _currentSpawnInterval = _initialSpawnInterval;
            _enemiesToSpawn = 1;

            _gameObjects.Add(Player);

            for (int i = 0; i < 1; i++)
            {
                SpawnAlien();
            }

            // Supply initialSupply = new Supply();
            // initialSupply.Load(_content);
            // _gameObjects.Add(initialSupply);
            // initialSupply.RandomMove();
            //
            // Vector2 asteroidPos1 = new Vector2(1000, 800);
            // Vector2 asteroidPos2 = new Vector2(-200, 200);
            // Vector2 asteroidPos3 = new Vector2(500, -400);
            //
            // Asteroid asteroid1 = new Asteroid(asteroidPos1);
            // Asteroid asteroid2 = new Asteroid(asteroidPos2);
            // Asteroid asteroid3 = new Asteroid(asteroidPos3);
            //
            // asteroid1.Load(_content);
            // asteroid2.Load(_content);
            // asteroid3.Load(_content);
            //
            // _gameObjects.Add(asteroid1);
            // _gameObjects.Add(asteroid2);
            // _gameObjects.Add(asteroid3);
        }

        public void SetGameState(GameState newState)
        {
            CurrentGameState = newState;
        }

        public void Load(ContentManager content)
        {
            _backgroundTexture = content.Load<Texture2D>("ZombieBackground");
            _titleFont = content.Load<SpriteFont>("TitleFont");
            _buttonFont = content.Load<SpriteFont>("ButtonFont");
            if (_hud == null)
                _hud = new HUD();

            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Load(content);
            }

            _hud.Load(content);
        }

        public void HandleInput(InputManager inputManager)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.HandleInput(this.InputManager);
            }
        }

        public void CheckCollision()
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                for (int j = i + 1; j < _gameObjects.Count; j++)
                {
                    if (_gameObjects[i].CheckCollision(_gameObjects[j]))
                    {
                        _gameObjects[i].OnCollision(_gameObjects[j]);
                        _gameObjects[j].OnCollision(_gameObjects[i]);
                    }
                }
            }
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(_camera.GetViewMatrix()));
        }

        public void Update(GameTime gameTime)
        {
            InputManager.Update();
            MouseState mouseState = Mouse.GetState();

            if (CurrentGameState == GameState.StartScreen)
            {
                _startButton.Update(mouseState);
                _quitButton.Update(mouseState);

                return;
            }

            if (CurrentGameState == GameState.Paused)
            {
                _continueButton.Update(mouseState);
                _pauseQuitButton.Update(mouseState);

                return;
            }

            if (CurrentGameState == GameState.GameOver)
            {
                _restartButton.Update(mouseState);
                _quitButton.Update(mouseState);

                return;
            }

            if (CurrentGameState == GameState.Playing)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _gameTimeElapsed += deltaTime;
                _spawnTimer += deltaTime;
                _supplySpawnTimer += deltaTime;

                UpdatePhase();
                SpawnEnemies();
                // SpawnSupply();

                _camera.Update(Player.GetPosition().Center.ToVector2());
                _hud.Update();

                HandleInput(InputManager);

                foreach (GameObject gameObject in _gameObjects)
                {
                    gameObject.Update(gameTime);
                }

                CheckCollision();

                foreach (GameObject gameObject in _toBeAdded)
                {
                    if (gameObject is Zombie)
                    {
                        Zombies.Add(gameObject as Zombie);
                    }

                    gameObject.Load(_content);
                    _gameObjects.Add(gameObject);
                }

                _toBeAdded.Clear();

                foreach (GameObject gameObject in _toBeRemoved)
                {
                    if (gameObject is Zombie)
                    {
                        Zombies.Remove(gameObject as Zombie);
                    }


                    if (gameObject is Friendly)
                    {
                        Friendlies.Remove(gameObject as Friendly);
                    }

                    _gameObjects.Remove(gameObject);
                }

                _toBeRemoved.Clear();
            }
        }

        private void UpdatePhase()
        {
            if (_gameTimeElapsed < 60f) // early game
            {
                _currentSpawnInterval = 3.0f;
                _enemiesToSpawn = 1;
                _maxEnemiesOnScreen = 20;
            }
            else if (_gameTimeElapsed < 180f) // mid game
            {
                _currentSpawnInterval = 2.0f;
                _enemiesToSpawn = 2;
                _maxEnemiesOnScreen = 35;
            }
            else // late game
            {
                _currentSpawnInterval = 1.2f;
                _enemiesToSpawn = 3;
                _maxEnemiesOnScreen = 50;
            }
        }

        private void SpawnEnemies()
        {
            if (_spawnTimer < _currentSpawnInterval)
                return;
            _spawnTimer = 0f;

            int currentAlienCount = _gameObjects.OfType<Zombie>().Count();

            if (currentAlienCount >= _maxEnemiesOnScreen)
                return;

            _enemiesToSpawn = Math.Min(_enemiesToSpawn, _maxEnemiesOnScreen - currentAlienCount);

            for (int i = 0; i < _enemiesToSpawn; i++)
                SpawnAlien();
        }

        private void SpawnAlien()
        {
            Zombie newZombie = new Zombie();
            newZombie.Load(_content);
            AddGameObject(newZombie);
            newZombie.RandomMove();
        }

        private void SpawnSupply()
        {
            if (_supplySpawnTimer < _supplySpawnInterval)
                return;

            _supplySpawnTimer = 0f;

            Supply newSupply = new Supply();
            AddGameObject(newSupply);
            newSupply.RandomMove();
        }

        public void AddScore(int pointsToAdd)
        {
            _score += pointsToAdd;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            if (CurrentGameState == GameState.Playing || CurrentGameState == GameState.Paused)
            {
                foreach (GameObject gameObject in _gameObjects)
                {
                    gameObject.Draw(gameTime, spriteBatch);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin();

            if (CurrentGameState == GameState.StartScreen)
            {
                spriteBatch.Draw(_backgroundTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    Color.White);

                var quitBounds = _quitButton.Rectangle;

                float spacing = 120f;

                float textY = quitBounds.Y + quitBounds.Height + spacing;

                string titleText = "The Cure";
                Vector2 titleSize = _titleFont.MeasureString(titleText);
                Vector2 titlePosition = new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2,
                    textY
                );

                spriteBatch.DrawString(_titleFont, titleText, titlePosition, Color.Red);

                _startButton.Draw(spriteBatch);
                _quitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.Paused)
            {
                spriteBatch.Draw(DummyTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    new Color(0, 0, 0, 128)
                );

                string pauseText = "Game gepauzeerd";
                Vector2 pauseTextSize = _titleFont.MeasureString(pauseText);
                Vector2 pauseTextPosition =
                    new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - pauseTextSize.X / 2, 150);

                spriteBatch.DrawString(_titleFont, pauseText, pauseTextPosition, Color.White);

                _continueButton.Draw(spriteBatch);
                _pauseQuitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.GameOver)
            {
                float spacing = 20f;
                float currentY = 150f;

                spriteBatch.Draw(DummyTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    new Color(0, 0, 0, 200)
                );

                string gameOverText = "Game Over";
                Vector2 gameOverTextSize = _titleFont.MeasureString(gameOverText);
                Vector2 gameOverTextPosition =
                    new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - gameOverTextSize.X / 2, currentY);

                spriteBatch.DrawString(_titleFont, gameOverText, gameOverTextPosition, Color.Red);

                currentY += gameOverTextSize.Y + spacing;

                string scoreText = $"Eindscore: {_score}";
                Vector2 scoreTextSize = _titleFont.MeasureString(scoreText);
                Vector2 scoreTextPosition =
                    new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - scoreTextSize.X / 2, currentY);

                spriteBatch.DrawString(_titleFont, scoreText, scoreTextPosition, Color.White);

                _restartButton.Draw(spriteBatch);
                _quitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.Playing)
            {
                _hud.Draw(spriteBatch, this);
            }

            spriteBatch.End();
        }

        public float GetGameTime()
        {
            return _gameTimeElapsed;
        }

        public int GetScore()
        {
            return _score;
        }

        public List<Stat> GetStats()
        {
            var stats = new List<Stat>
            {
                new Stat("Max Health", Player.MaxHealth.ToString()),
                new Stat("Move Speed", (Player.MoveSpeed / 10).ToString("0.0", CultureInfo.InvariantCulture)),
                new Stat("Friendlies", _gameObjects.OfType<Friendly>().Count().ToString())
            };
            return stats;
        }

        public void AddGameObject(GameObject gameObject)
        {
            _toBeAdded.Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            _toBeRemoved.Add(gameObject);
        }

        public Vector2 RandomScreenLocation()
        {
            return new Vector2(
                RNG.Next(0, Game.GraphicsDevice.Viewport.Width),
                RNG.Next(0, Game.GraphicsDevice.Viewport.Height)
            );
        }

        public Vector2 RandomLocationOutsideView(int margin = 150)
        {
            if (_camera == null)
                return RandomScreenLocation();

            Rectangle viewBounds = _camera.GetViewBounds();
            int left = viewBounds.Left - margin;
            int right = viewBounds.Right + margin;
            int top = viewBounds.Top - margin;
            int bottom = viewBounds.Bottom + margin;

            int side = RNG.Next(0, 4);
            switch (side)
            {
                case 0: // Spawn boven uit het zicht
                    return new Vector2(RNG.Next(left, right), RNG.Next(top, viewBounds.Top));
                case 1: // Spawn onder uit het zicht
                    return new Vector2(RNG.Next(left, right), RNG.Next(viewBounds.Bottom, bottom));
                case 2: // Spawn links uit het zicht
                    return new Vector2(RNG.Next(left, viewBounds.Left), RNG.Next(top, bottom));
                default: // Spawn rechts uit het zicht
                    return new Vector2(RNG.Next(viewBounds.Right, right), RNG.Next(top, bottom));
            }
        }
    }
}