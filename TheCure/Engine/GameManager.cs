using System;
using System.Collections.Generic;
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
        private Planet _pickupPlanet;
        private Planet _dropOffPlanet;
        private Camera _camera;
        private int _score = 0;
        private SpriteFont _hudFont;

        private float _spawnTimer = 0f;
        private float _timeSinceLastSpawn = 0f;
        private float _initialSpawnInterval = 5.0f;
        private float _currentSpawnInterval;
        private float _minimumSpawnInterval = 1.0f;
        private float _difficultyRampFactor = 0.98f;
        private int _enemiesToSpawn = 1;
        private int _maxEnemiesPerSpawn = 5;
        private float _increaseSpawnCountInterval = 30.0f;
        private float _timeSinceLastSpawnCountIncrease = 0f;
        private float _supplySpawnTimer = 0f;
        private float _supplySpawnInterval = 15.0f;

        public Random RNG
        {
            get;
            private set;
        }
        public Ship Player
        {
            get;
            private set;
        }
        public InputManager InputManager
        {
            get;
            private set;
        }
        public Game Game
        {
            get;
            private set;
        }
        public Texture2D DummyTexture
        {
            get;
            private set;
        }
        public GameState CurrentGameState
        {
            get;
            private set;
        }

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

            InputManager = new InputManager();
            RNG = new Random();

            CurrentGameState = GameState.StartScreen;
            _currentSpawnInterval = _initialSpawnInterval;
        }

        public void Initialize(ContentManager content, Game game, Ship player)
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
                new Rectangle(centerX - buttonWidth / 2, centerY - buttonHeight - spacing / 2, buttonWidth, buttonHeight),
                "Start",
                _buttonFont);
            _startButton.Clicked += StartButton_Clicked;

            _quitButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, centerY + spacing / 2, buttonWidth, buttonHeight),
                "Quit",
                _buttonFont);
            _quitButton.Clicked += QuitButton_Clicked;

            _continueButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, (int)(centerY - buttonHeight - spacing * 1.5f), buttonWidth, buttonHeight),
                "Continue",
                _buttonFont);
            _continueButton.Clicked += ContinueButton_Clicked;

            _pauseQuitButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, (int)(centerY + spacing * 0.5f), buttonWidth, buttonHeight),
                "Quit",
                _buttonFont);
            _pauseQuitButton.Clicked += PauseQuitButton_Clicked;

            _restartButton = new Button(
                new Rectangle(centerX - buttonWidth / 2, centerY - spacing, buttonWidth, buttonHeight),
                "Opnieuw spelen",
                _buttonFont);
            _restartButton.Clicked += RestartButton_Clicked;

            Vector2 pickupPos = new Vector2(-500, -300);
            Vector2 dropOffPos = new Vector2(2000, 600);

            _pickupPlanet = new Planet(pickupPos, PlanetType.Pickup);
            _dropOffPlanet = new Planet(dropOffPos, PlanetType.DropOff);

            _gameObjects.Add(_pickupPlanet);
            _gameObjects.Add(_dropOffPlanet);

            _pickupPlanet.Load(content);
            _dropOffPlanet.Load(content);

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

            Player._health = Ship.MaxHealth;
            Player._isCarryingCargo = false;
            Player._currentWeapon = Player._bulletWeapon;
            Player._weaponBuffTimer = 0f;
            Player._rectangleCollider.shape.Location = new Point(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            Player._velocity = Vector2.Zero;
            Player._rotation = 0f;

            _spawnTimer = 0f;
            _timeSinceLastSpawn = 0f;
            _timeSinceLastSpawnCountIncrease = 0f;
            _supplySpawnTimer = 0f;
            _currentSpawnInterval = _initialSpawnInterval;
            _enemiesToSpawn = 1;

            _gameObjects.Add(Player);
            _gameObjects.Add(_pickupPlanet);
            _gameObjects.Add(_dropOffPlanet);

            for (int i = 0; i < 1; i++)
            {
                Alien newAlien = new Alien();
                newAlien.Load(_content);
                _gameObjects.Add(newAlien);
                newAlien.RandomMove();
            }

            Supply initialSupply = new Supply();
            initialSupply.Load(_content);
            _gameObjects.Add(initialSupply);
            initialSupply.RandomMove();

            Vector2 asteroidPos1 = new Vector2(1000, 800);
            Vector2 asteroidPos2 = new Vector2(-200, 200);
            Vector2 asteroidPos3 = new Vector2(500, -400);

            Asteroid asteroid1 = new Asteroid(asteroidPos1);
            Asteroid asteroid2 = new Asteroid(asteroidPos2);
            Asteroid asteroid3 = new Asteroid(asteroidPos3);

            asteroid1.Load(_content);
            asteroid2.Load(_content);
            asteroid3.Load(_content);

            _gameObjects.Add(asteroid1);
            _gameObjects.Add(asteroid2);
            _gameObjects.Add(asteroid3);
        }

        public void SetGameState(GameState newState)
        {
            CurrentGameState = newState;
        }

        public void Load(ContentManager content)
        {
            _backgroundTexture = content.Load<Texture2D>("StartBackground");
            _titleFont = content.Load<SpriteFont>("TitleFont");
            _buttonFont = content.Load<SpriteFont>("ButtonFont");
            _hudFont = content.Load<SpriteFont>("HudFont");

            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Load(content);
            }
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

                _spawnTimer += deltaTime;
                _timeSinceLastSpawn += deltaTime;
                _timeSinceLastSpawnCountIncrease += deltaTime;
                _supplySpawnTimer += deltaTime;

                if (_supplySpawnTimer >= _supplySpawnInterval)
                {
                    Supply newSupply = new Supply();
                    AddGameObject(newSupply);
                    newSupply.RandomMove();
                    _supplySpawnTimer = 0f;
                }

                if (_timeSinceLastSpawn >= _currentSpawnInterval)
                {
                    for (int i = 0; i < _enemiesToSpawn; i++)
                    {
                        Alien newAlien = new Alien();
                        newAlien.Load(_content);
                        AddGameObject(newAlien);
                        newAlien.RandomMove();
                    }
                    _timeSinceLastSpawn = 0f;

                    _currentSpawnInterval *= _difficultyRampFactor;
                    if (_currentSpawnInterval < _minimumSpawnInterval)
                    {
                        _currentSpawnInterval = _minimumSpawnInterval;
                    }

                    if (_timeSinceLastSpawnCountIncrease >= _increaseSpawnCountInterval && _enemiesToSpawn < _maxEnemiesPerSpawn)
                    {
                        _enemiesToSpawn++;
                        _timeSinceLastSpawnCountIncrease = 0f;
                    }
                }

                _camera.Update(Player.GetPosition().Center.ToVector2());

                HandleInput(InputManager);

                foreach (GameObject gameObject in _gameObjects)
                {
                    gameObject.Update(gameTime);
                }

                CheckCollision();

                foreach (GameObject gameObject in _toBeAdded)
                {
                    gameObject.Load(_content);
                    _gameObjects.Add(gameObject);
                }

                _toBeAdded.Clear();

                foreach (GameObject gameObject in _toBeRemoved)
                {
                    gameObject.Destroy();
                    _gameObjects.Remove(gameObject);
                }

                _toBeRemoved.Clear();
            }
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
                spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);

                string titleText = "Space Defence";
                Vector2 titleSize = _titleFont.MeasureString(titleText);
                Vector2 titlePosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2, 100);

                spriteBatch.DrawString(_titleFont, titleText, titlePosition, Color.White);

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
                Vector2 pauseTextPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - pauseTextSize.X / 2, 150);

                spriteBatch.DrawString(_titleFont, pauseText, pauseTextPosition, Color.White);

                _continueButton.Draw(spriteBatch);
                _pauseQuitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.GameOver)
            {
                spriteBatch.Draw(DummyTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    new Color(0, 0, 0, 200)
                );

                string gameOverText = "Game Over";
                Vector2 gameOverTextSize = _titleFont.MeasureString(gameOverText);
                Vector2 gameOverTextPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - gameOverTextSize.X / 2, 150);

                spriteBatch.DrawString(_titleFont, gameOverText, gameOverTextPosition, Color.Red);

                string scoreText = $"Eindscore: {_score}";
                Vector2 scoreTextSize = _titleFont.MeasureString(scoreText);
                Vector2 scoreTextPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - scoreTextSize.X / 2, 200);

                spriteBatch.DrawString(_titleFont, scoreText, scoreTextPosition, Color.White);

                _restartButton.Draw(spriteBatch);
                _quitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.Playing)
            {
                if (_hudFont != null && Player != null)
                {
                    string scoreText = $"Score: {_score}";
                    Vector2 scorePosition = new Vector2(10, 10);
                    spriteBatch.DrawString(_hudFont, scoreText, scorePosition, Color.White);

                    string cargoText = $"Cargo: " + (Player.IsCarryingCargo ? "Ja" : "Nee");
                    Vector2 cargoPosition = new Vector2(10, 35);
                    spriteBatch.DrawString(_hudFont, cargoText, cargoPosition, Color.White);

                    int barWidth = 150;
                    int barHeight = 15;
                    Vector2 barPosition = new Vector2(10, 60);
                    spriteBatch.Draw(DummyTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);
                    float healthRatio = Player.Health / Ship.MaxHealth;
                    spriteBatch.Draw(DummyTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthRatio), barHeight), Color.Green);
                }
            }

            spriteBatch.End();
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