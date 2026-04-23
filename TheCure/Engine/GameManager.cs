using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Engine.Managers;
using TheCure.Managers;
using TheCure.Mobs;

namespace TheCure
{
    public class GameManager : Manager<GameManager>
    {
        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        private Camera _camera;

        private float _gameTimeElapsed = 0f;
        private float _spawnTimer = 0f;

        private float _initialSpawnInterval = 5.0f;
        private float _currentSpawnInterval;

        private int _enemiesToSpawn = 1;
        private int _maxEnemiesOnScreen;
        private int _maxBrutesOnScreen;
        private float _bruteSpawnChance;




        //World borders
        private const int WorldWidth = 3600;
        private const int WorldHeight = 2400;
        private const int WallThickness = 32;
        private readonly Rectangle _playableBounds = new Rectangle(-1800, -1200, WorldWidth, WorldHeight);


        public Random RNG { get; private set; }
        public Game Game { get; private set; }
        public GameState CurrentGameState { get; private set; }
        public HUD HUD { get; private set; }
        public List<Mob> Enemies;
        public Camera Camera => _camera;

        public List<Friendly> Friendlies { get; private set; } = new List<Friendly>();

        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            Enemies = new List<Mob>();

            RNG = new Random();

            CurrentGameState = GameState.StartScreen;
            _currentSpawnInterval = _initialSpawnInterval;
        }

        public void Initialize(Game game)
        {
            Game = game;
            _camera = new Camera(Game.GraphicsDevice.Viewport);
            CurrentGameState = GameState.StartScreen;
            AddGameObject(PlayerManager.Get().Player);
            AddWorldWalls();
        }

        public void ResetGame()
        {
            _gameObjects.Clear();
            _toBeRemoved.Clear();
            _toBeAdded.Clear();
            Friendlies.Clear();

            ScoreManager.Get().Reset();
            PlayerManager.Get().ResetPlayer();
            BoostManager.Get().Reset();

            _gameTimeElapsed = 0f;
            _spawnTimer = 0f;
            _currentSpawnInterval = _initialSpawnInterval;
            _enemiesToSpawn = 1;


            PlayerActionsManager.Get().Reset();
            UpgradeManager.Get().Reset();

            AddWorldWalls();
            _gameObjects.Add(PlayerManager.Get().Player);

            for (var i = 0; i < 1; i++)
            {
                SpawnZombie();
            }
        }

        public void SetGameState(GameState newState)
        {
            CurrentGameState = newState;
        }

        public void Load()
        {
            HUD = new HUD();

            foreach (var gameObject in _gameObjects)
            {
                gameObject.Load();
            }

            PlayerActionsManager.Get().Load();
            HUD.Load();
        }

        public void HandleInput()
        {
            foreach (var gameObject in _gameObjects)
            {
                gameObject.HandleInput();
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
            PlayerActionsManager.Get().Update(gameTime);
            UpgradeManager.Get().Update(gameTime);
            ScoreManager.Get().Update(gameTime);
            BoostManager.Get().Update(gameTime);

            if (CurrentGameState == GameState.Upgrade)
            {
                UpgradeManager.Get().UpdateButtons(gameTime);
                return;
            }

            if (CurrentGameState == GameState.Playing)
            {
                var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _gameTimeElapsed += deltaTime;
                _spawnTimer += deltaTime;

                UpdatePhase();
                SpawnEnemies();
                HandleInput();

                foreach (var gameObject in _gameObjects)
                {
                    gameObject.Update(gameTime);
                }

                _camera.Update(PlayerManager.Get().Player.GetPosition().Center.ToVector2(), GetWorldBounds());
                HUD.Update(gameTime);

                CheckCollision();

                foreach (var gameObject in _toBeAdded)
                {
                    gameObject.Load();

                    if (gameObject is Zombie zombie)
                    {
                        zombie.RandomMove();
                        Enemies.Add(zombie);
                    }

                    if (gameObject is Brute brute)
                    {
                        brute.RandomMove();
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
            AddGameObject(newZombie);
        }

        private void SpawnBrute()
        {
            Brute brute = new Brute();
            AddGameObject(brute);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (CurrentGameState)
            {
                case GameState.StartScreen:
                    ScreenManager.Get().DrawStartScreen(spriteBatch);
                    break;

                case GameState.Playing:
                    DrawGameObjects(spriteBatch, gameTime);

                    spriteBatch.Begin();
                    HUD.Draw(spriteBatch, this);
                    PlayerActionsManager.Get().Draw(spriteBatch);
                    spriteBatch.End();
                    break;

                case GameState.Upgrade:
                    DrawGameObjects(spriteBatch, gameTime);

                    spriteBatch.Begin();
                    HUD.Draw(spriteBatch, this);
                    PlayerActionsManager.Get().Draw(spriteBatch);

                    UpgradeManager.Get().Draw(spriteBatch, this);
                    spriteBatch.End();
                    break;

                case GameState.Paused:
                    ScreenManager.Get().DrawPauseMenu(spriteBatch);
                    break;

                case GameState.GameOver:
                    ScreenManager.Get().DrawGameOver(spriteBatch);
                    break;
            }
        }

        public float GetGameTime()
        {
            return _gameTimeElapsed;
        }

        public List<Stat> GetStats()
        {
            var stats = new List<Stat>
            {
                new Stat("Max Health", PlayerManager.Get().Player.MaxHealth.ToString()),
                new Stat("Move Speed",
                    (PlayerManager.Get().Player.MoveSpeed / 10).ToString("0.0", CultureInfo.InvariantCulture)),
                new Stat("Friendlies", _gameObjects.OfType<Friendly>().Count().ToString()),
            };
            foreach (var boost in BoostManager.Get()._boosts)
            {
                stats.Add(new Stat("Boost", $"x{boost.GetUnlockedBoostMultiplier():0.00}"));
            }
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

            Vector2 playerPos = PlayerManager.Get().Player.GetPosition().Center.ToVector2();
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
            wall.Load();
            _gameObjects.Add(wall);
        }

        private void DrawGameObjects(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.LinearClamp);

            DrawTiledGameplayBackground(spriteBatch);
            foreach (var gameObject in _gameObjects)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
        }

        private void DrawTiledGameplayBackground(SpriteBatch spriteBatch)
        {
            var content = ContentsManager.Get();
            var worldBounds = GetWorldBounds();

            for (var x = worldBounds.Left; x < worldBounds.Right; x += content.BackgroundGamePlayTexture.Width)
            {
                for (var y = worldBounds.Top; y < worldBounds.Bottom; y += content.BackgroundGamePlayTexture.Height)
                {
                    var tileWidth = Math.Min(content.BackgroundGamePlayTexture.Width, worldBounds.Right - x);
                    var tileHeight = Math.Min(content.BackgroundGamePlayTexture.Height, worldBounds.Bottom - y);

                    spriteBatch.Draw(
                        content.BackgroundGamePlayTexture,
                        new Rectangle(x, y, tileWidth, tileHeight),
                        new Rectangle(0, 0, tileWidth, tileHeight),
                        Color.White);
                }
            }
        }
    }
}