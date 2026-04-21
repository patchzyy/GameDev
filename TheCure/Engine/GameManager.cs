using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Weapons;
using TheCure.Mobs;
using TheCure.World;

namespace TheCure
{
    public class GameManager
    {
        private static GameManager gameManager;
        private ScoreManager scoreManager;

        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        public ContentManager _content;
        private Texture2D _backgroundTexture;
        private Texture2D _backgroundPauseTexture;
        private Texture2D _backgroundGameOverTexture;
        private Texture2D _gameplayBackgroundTexture;
        private SpriteFont _titleFont;
        private SpriteFont _buttonFont;
        private Button _startButton;
        private Button _quitButton;
        private Button _continueButton;
        private Button _pauseQuitButton;
        private Button _restartButton;
        private Camera _camera;
        private List<ScorePopup> _scorePopups = new List<ScorePopup>();

        private float _gameTimeElapsed = 0f;
        private float _spawnTimer = 0f;

        private float _initialSpawnInterval = 5.0f;
        private float _currentSpawnInterval;

        private int _enemiesToSpawn = 1;
        private int _maxEnemiesOnScreen;
        private int _maxBrutesOnScreen;
        private float _bruteSpawnChance;

        private float _supplySpawnTimer = 0f;
        private float _supplySpawnInterval = 15.0f;


        //World borders
        private const int WorldWidth = 3600;
        private const int WorldHeight = 2400;
        private const int WallThickness = 32;
        private readonly Rectangle _playableBounds = new Rectangle(-1800, -1200, WorldWidth, WorldHeight);


        public Random RNG { get; private set; }
        public Player Player { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }
        public Texture2D DummyTexture { get; private set; }
        public GameState CurrentGameState { get; private set; }
        public HUD HUD { get; private set; }
        public List<Mob> Enemies;
        public Camera Camera => _camera;


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
            Enemies = new List<Mob>();

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

            DummyTexture = new(Game.GraphicsDevice, 1, 1);
            DummyTexture.SetData(new[] { Color.White });

            CreateButtons();

            PositionButtons();

            CurrentGameState = GameState.StartScreen;
        }

        private void CreateButtons()
        {
            int buttonWidth = 200;
            int buttonHeight = 50;
            _startButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Start", _buttonFont);
            _quitButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Quit", _buttonFont);
            _continueButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Continue", _buttonFont);
            _pauseQuitButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Quit", _buttonFont);
            _restartButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Opnieuw spelen", _buttonFont);

            _startButton.Clicked += StartButton_Clicked;
            _quitButton.Clicked += QuitButton_Clicked;
            _continueButton.Clicked += ContinueButton_Clicked;
            _pauseQuitButton.Clicked += PauseQuitButton_Clicked;
            _restartButton.Clicked += RestartButton_Clicked;
        }

        private void PositionButtons()
        {
            int buttonWidth = 200;
            int centerX = Game.GraphicsDevice.Viewport.Width / 2;

            _startButton.SetPosition(centerX - buttonWidth / 2, (int)(Game.GraphicsDevice.Viewport.Height * 0.54f));
            _quitButton.SetPosition(centerX - buttonWidth / 2, (int)(Game.GraphicsDevice.Viewport.Height * 0.68f));
            _continueButton.SetPosition(centerX - buttonWidth / 2, (int)(Game.GraphicsDevice.Viewport.Height * 0.5f));
            _restartButton.SetPosition(centerX - buttonWidth / 2, (int)(Game.GraphicsDevice.Viewport.Height * 0.5f));
            _pauseQuitButton.SetPosition(centerX - buttonWidth / 2, (int)(Game.GraphicsDevice.Viewport.Height * 0.68f));
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
            Friendlies.Clear();

            scoreManager.Reset();

            Player.GainHealth((int)Player.MaxHealth);
            Player.WeaponsSystem = new WeaponsSystem();
            Player._rectangleCollider.shape.Location = new Point(Game.GraphicsDevice.Viewport.Width / 2,
                Game.GraphicsDevice.Viewport.Height / 2);
            Player._velocity = Vector2.Zero;
            Player._rotation = 0f;

            _gameTimeElapsed = 0f;
            _spawnTimer = 0f;
            _supplySpawnTimer = 0f;
            _currentSpawnInterval = _initialSpawnInterval;
            _enemiesToSpawn = 1;

            HUD.Reset();

            AddWorldWalls();
            _gameObjects.Add(Player);

            for (var i = 0; i < 1; i++)
            {
                SpawnZombie();
            }
        }

        public void SetGameState(GameState newState)
        {
            CurrentGameState = newState;
        }

        public void Load(ContentManager content)
        {
            _backgroundTexture = content.Load<Texture2D>("ZombieBackground");
            _gameplayBackgroundTexture = content.Load<Texture2D>("BackGround");
            _backgroundPauseTexture = content.Load<Texture2D>("BackgroundPause");
            _backgroundGameOverTexture = content.Load<Texture2D>("GameOverBackground");
            _titleFont = content.Load<SpriteFont>("TitleFont");
            _buttonFont = content.Load<SpriteFont>("ButtonFont");
            HUD = new HUD();
            scoreManager = new ScoreManager();

            foreach (var gameObject in _gameObjects)
            {
                gameObject.Load(content);
            }

            HUD.Load(content);
        }

        public void HandleInput(InputManager inputManager)
        {
            foreach (var gameObject in _gameObjects)
            {
                gameObject.HandleInput(this.InputManager);
            }
        }

        public void CheckCollision()
        {
            for (var i = 0; i < _gameObjects.Count; i++)
            {
                for (var j = i + 1; j < _gameObjects.Count; j++)
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
            var mouseState = Mouse.GetState();

            if (CurrentGameState == GameState.StartScreen)
            {
                _startButton.Update(mouseState);
                _quitButton.Update(mouseState);

                return;
            }

            if (CurrentGameState == GameState.Paused)
            {
                if (InputManager.IsKeyPress(Keys.Space))
                {
                    CurrentGameState = GameState.Playing;
                }

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
                var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _gameTimeElapsed += deltaTime;
                _spawnTimer += deltaTime;
                _supplySpawnTimer += deltaTime;

                UpdatePhase();
                SpawnEnemies();
                // SpawnSupply();
                for (int i = _scorePopups.Count - 1; i >= 0; i--)
                {
                    _scorePopups[i].TimeLeft -= deltaTime;

                    if (_scorePopups[i].TimeLeft <= 0)
                    {
                        _scorePopups.RemoveAt(i);
                    }
                }

                HandleInput(InputManager);

                foreach (var gameObject in _gameObjects)
                {
                    gameObject.Update(gameTime);
                }

                _camera.Update(Player.GetPosition().Center.ToVector2(), GetWorldBounds());
                HUD.Update(gameTime);

                CheckCollision();

                foreach (var gameObject in _toBeAdded)
                {
                    gameObject.Load(_content);

                    if (gameObject is Zombie zombie)
                    {
                        Enemies.Add(zombie);
                    }

                    if (gameObject is Brute brute)
                    {
                        Enemies.Add(brute);
                    }

                    _gameObjects.Add(gameObject);
                }

                _toBeAdded.Clear();

                foreach (var gameObject in _toBeRemoved)
                {
                    if (gameObject is Mob)
                    {
                        Enemies.Remove(gameObject as Mob);
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
                _currentSpawnInterval = Settings.GetValue(SettingsConst.SPAWNING.ZOMBIE_SPAWN_INTERVAL);
                _enemiesToSpawn = Settings.GetValue(SettingsConst.SPAWNING.ENEMIES_PER_WAVE);
                _maxEnemiesOnScreen = Settings.GetValue(SettingsConst.SPAWNING.MAX_ENEMIES_ON_SCREEN);
                _maxBrutesOnScreen = Settings.GetValue(SettingsConst.SPAWNING.MAX_BRUTES);
                _bruteSpawnChance = Settings.GetValue(SettingsConst.SPAWNING.BRUTE_SPAWN_CHANCE);
            }
            else if (_gameTimeElapsed < 180f) // mid game
            {
                _currentSpawnInterval = 2.0f;
                _enemiesToSpawn = 2;
                _maxEnemiesOnScreen = 35;
                _maxBrutesOnScreen = 2;
                _bruteSpawnChance = 0.15f;
            }
            else // late game
            {
                _currentSpawnInterval = 1.2f;
                _enemiesToSpawn = 3;
                _maxEnemiesOnScreen = 50;
                _maxBrutesOnScreen = 5;
                _bruteSpawnChance = 0.20f;
            }
        }

        private void SpawnEnemies()
        {
            if (_spawnTimer < _currentSpawnInterval)
                return;

            _spawnTimer = 0f;

            int zombieCount = _gameObjects.OfType<Zombie>().Count();
            int bruteCount = _gameObjects.OfType<Brute>().Count();
            int totalEnemies = zombieCount + bruteCount;

            if (totalEnemies >= _maxEnemiesOnScreen)
                return;

            _enemiesToSpawn = Math.Min(_enemiesToSpawn, _maxEnemiesOnScreen - totalEnemies);

            for (var i = 0; i < _enemiesToSpawn; i++)
            {
                if (bruteCount < _maxBrutesOnScreen && RNG.NextDouble() < _bruteSpawnChance)
                {
                    SpawnBrute();
                    bruteCount++;
                }
                else
                {
                    SpawnZombie();
                    zombieCount++;
                }
            }
        }

        private void SpawnZombie()
        {
            var newZombie = new Zombie();
            Vector2 spawnPos = RandomLocationOutsideView();
            newZombie.Spawn(spawnPos);
            AddGameObject(newZombie);
        }

        private void SpawnBrute()
        {
            Brute brute = new Brute();
            Vector2 spawnPos = RandomLocationOutsideView();
            brute.Spawn(spawnPos);
            AddGameObject(brute);
        }

        private void SpawnSupply()
        {
            if (_supplySpawnTimer < _supplySpawnInterval)
                return;

            _supplySpawnTimer = 0f;

            var newSupply = new Supply();
            newSupply.Load(_content);
            AddGameObject(newSupply);
            newSupply.RandomMove();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.LinearClamp);

            if (CurrentGameState == GameState.Playing || CurrentGameState == GameState.Paused)
            {
                DrawTiledGameplayBackground(spriteBatch);

                foreach (var gameObject in _gameObjects)
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

                var titleText = "The Cure";
                var titleSize = _titleFont.MeasureString(titleText);

                var titlePosition = new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2,
                    Game.GraphicsDevice.Viewport.Height / 8f
                );

                spriteBatch.DrawString(_titleFont, titleText, titlePosition, Color.Red);

                _startButton.Draw(spriteBatch);
                _quitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.Paused)
            {
                spriteBatch.Draw(_backgroundPauseTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    Color.White);

                spriteBatch.Draw(DummyTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    new Color(0, 0, 0, 100));

                string pauseText = "Game gepauzeerd";
                Vector2 pauseTextSize = _titleFont.MeasureString(pauseText);
                float scale = 0.6f;
                Vector2 pauseTextPosition =
                    new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - (pauseTextSize.X * scale) / 2,
                        Game.GraphicsDevice.Viewport.Height / 8f);

                spriteBatch.DrawString(_titleFont, pauseText, pauseTextPosition, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);

                _continueButton.Draw(spriteBatch);
                _pauseQuitButton.Draw(spriteBatch);
            }
            else if (CurrentGameState == GameState.GameOver)
            {
                var spacing = 20f;
                var currentY = 150f;

                spriteBatch.Draw(_backgroundGameOverTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    Color.White);

                spriteBatch.Draw(DummyTexture,
                    new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                    new Color(0, 0, 0, 100));

                var gameOverText = "Game Over";
                var gameOverTextSize = _titleFont.MeasureString(gameOverText);
                var gameOverTextPosition =
                    new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - gameOverTextSize.X / 2,
                        Game.GraphicsDevice.Viewport.Height / 8f);

                spriteBatch.DrawString(_titleFont, gameOverText, gameOverTextPosition, Color.Red);

                currentY += gameOverTextSize.Y + spacing;

                string scoreText = $"Eindscore: {GetScore()}";
                Vector2 scoreTextSize = _titleFont.MeasureString(scoreText);
                float scale = 0.5f;
                Vector2 scoreTextPosition =
                    new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - (scoreTextSize.X * scale) / 2,
                        Game.GraphicsDevice.Viewport.Height / 10f);

                spriteBatch.DrawString(_titleFont, scoreText, scoreTextPosition, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);

                _restartButton.Draw(spriteBatch);
                _quitButton.Draw(spriteBatch);
            }

            if (CurrentGameState == GameState.Playing || CurrentGameState == GameState.Paused ||
                CurrentGameState == GameState.GameOver)
            {
                HUD.Draw(spriteBatch, this);
            }

            spriteBatch.End();
        }

        public float GetGameTime()
        {
            return _gameTimeElapsed;
        }

        public int GetScore()
        {
            return scoreManager.GetScore();
        }

        public List<ScorePopup> GetScorePopups()
        {
            return _scorePopups;
        }

        public void AddScore(int pointsToAdd, string reason = "")
        {
            scoreManager.AddScore(pointsToAdd);
            _scorePopups.Add(new ScorePopup($"{reason}  +{pointsToAdd}", 2f));
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

        public Vector2 RandomLocationOutsideView(int margin = 150, int padding = 0)
        {
            if (_camera == null)
                return ClampToPlayableBounds(RandomScreenLocation(), padding);

            var playableBounds = GetPlayableBounds();
            var safePlayableBounds = new Rectangle(
                playableBounds.Left + padding,
                playableBounds.Top + padding,
                playableBounds.Width - padding * 2,
                playableBounds.Height - padding * 2);

            var blockedViewBounds = _camera.GetViewBounds();
            blockedViewBounds.Inflate(margin, margin);

            Vector2 playerPos = Player.GetPosition().Center.ToVector2();
            float minDistanceFromPlayer = margin;

            // todo: check of dit... niet random kan, heb dit van de les lol
            for (var i = 0; i < 20; i++)
            {
                var candidate = new Vector2(
                    RNG.Next(safePlayableBounds.Left, safePlayableBounds.Right),
                    RNG.Next(safePlayableBounds.Top, safePlayableBounds.Bottom));

                if (blockedViewBounds.Contains(candidate))
                    continue;

                if (Vector2.Distance(candidate, playerPos) < minDistanceFromPlayer)
                    continue;

                return candidate;
            }

            Vector2[] fallbackPoints =
            {
                new(safePlayableBounds.Left, safePlayableBounds.Top),
                new(safePlayableBounds.Right - 1, safePlayableBounds.Top),
                new(safePlayableBounds.Left, safePlayableBounds.Bottom - 1),
                new(safePlayableBounds.Right - 1, safePlayableBounds.Bottom - 1),
                new(safePlayableBounds.Center.X, safePlayableBounds.Top),
                new(safePlayableBounds.Center.X, safePlayableBounds.Bottom - 1),
                new(safePlayableBounds.Left, safePlayableBounds.Center.Y),
                new(safePlayableBounds.Right - 1, safePlayableBounds.Center.Y)
            };

            var bestPoint = fallbackPoints[0];
            var bestDistance = -1f;

            // verste hoek
            foreach (var point in fallbackPoints)
            {
                if (blockedViewBounds.Contains(point))
                    continue;

                if (Vector2.Distance(point, playerPos) < minDistanceFromPlayer)
                    continue;

                var distance = Vector2.DistanceSquared(point, playerPos);
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = point;
                }
            }

            return bestPoint;
        }

        public Rectangle GetPlayableBounds()
        {
            return _playableBounds;
        }

        public Rectangle GetWorldBounds()
        {
            return new Rectangle(
                _playableBounds.X - WallThickness,
                _playableBounds.Y - WallThickness,
                _playableBounds.Width + WallThickness * 2,
                _playableBounds.Height + WallThickness * 2);
        }

        public Vector2 ClampToPlayableBounds(Vector2 position, float padding = 0f)
        {
            var playableBounds = GetPlayableBounds();

            return new Vector2(
                MathHelper.Clamp(position.X, playableBounds.Left + padding, playableBounds.Right - padding),
                MathHelper.Clamp(position.Y, playableBounds.Top + padding, playableBounds.Bottom - padding));
        }

        public void AddWorldWalls()
        {
            // boven
            AddWorldWall(new(_playableBounds.Left - WallThickness, _playableBounds.Top - WallThickness,
                _playableBounds.Width + WallThickness * 2, WallThickness));

            // onder
            AddWorldWall(new(_playableBounds.Left - WallThickness, _playableBounds.Bottom,
                _playableBounds.Width + WallThickness * 2, WallThickness));

            // links
            AddWorldWall(new(_playableBounds.Left - WallThickness, _playableBounds.Top, WallThickness,
                _playableBounds.Height));

            // rechts
            AddWorldWall(new(_playableBounds.Right, _playableBounds.Top, WallThickness, _playableBounds.Height));
        }

        private void AddWorldWall(Rectangle bounds)
        {
            var wall = new Wall(bounds);

            if (_content != null && _backgroundTexture != null)
            {
                wall.Load(_content);
            }

            _gameObjects.Add(wall);
        }

        private void DrawTiledGameplayBackground(SpriteBatch spriteBatch)
        {
            if (_gameplayBackgroundTexture == null)
                return;

            var worldBounds = GetWorldBounds();

            for (var x = worldBounds.Left; x < worldBounds.Right; x += _gameplayBackgroundTexture.Width)
            {
                for (var y = worldBounds.Top; y < worldBounds.Bottom; y += _gameplayBackgroundTexture.Height)
                {
                    var tileWidth = Math.Min(_gameplayBackgroundTexture.Width, worldBounds.Right - x);
                    var tileHeight = Math.Min(_gameplayBackgroundTexture.Height, worldBounds.Bottom - y);

                    spriteBatch.Draw(
                        _gameplayBackgroundTexture,
                        new Rectangle(x, y, tileWidth, tileHeight),
                        new Rectangle(0, 0, tileWidth, tileHeight),
                        Color.White);
                }
            }
        }
    }
}