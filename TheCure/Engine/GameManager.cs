using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Engine.Managers;
using TheCure.Managers;
using TheCure.Enemies;

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

        private bool IsGameplayRunning => CurrentGameState == GameState.Playing;

        private float _initialSpawnInterval = 5.0f;
        private float _currentSpawnInterval;
        public readonly List<Rectangle> WorldObstacleBounds = new();

        private int _enemiesToSpawn = 1;
        private int _maxEnemiesOnScreen;
        private int _maxBrutesOnScreen;
        private float _bruteSpawnChance;
        private float _babyZombieSpawnChance;

        private const int WorldWidth = 3600;
        private const int WorldHeight = 2400;
        private const int WallThickness = 32;
        private readonly Rectangle _playableBounds = new Rectangle(-1800, -1200, WorldWidth, WorldHeight);

        public Random RNG { get; private set; }
        public Game Game { get; private set; }
        public GameState CurrentGameState { get; private set; }
        public GameState PreviousGameState { get; private set; }
        public HUD HUD { get; private set; }
        public List<Enemy> Enemies;
        public Camera Camera => _camera;

        public List<Friendly> Friendlies { get; private set; } = new List<Friendly>();

        public GameState ResumeState { get; private set; }

        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            Enemies = new List<Enemy>();

            RNG = new Random();

            CurrentGameState = GameState.HealSelection;
            PreviousGameState = GameState.HealSelection;
            _currentSpawnInterval = _initialSpawnInterval;
        }

        public void Initialize(Game game)
        {
            Game = game;
            _camera = new Camera(Game.GraphicsDevice);
            AddWorldWalls();
            GenerateWorldObjects();
            _gameObjects.Add(PlayerManager.Get().Player);
        }

        public void ResetGame()
        {
            _gameObjects.Clear();
            _toBeRemoved.Clear();
            _toBeAdded.Clear();
            Friendlies.Clear();
            Enemies.Clear();

            ScoreManager.Get().Reset();
            PlayerManager.Get().ResetPlayer();
            BoostManager.Get().Reset();
            CommandManager.Get().Reset();

            _gameTimeElapsed = 0f;
            _spawnTimer = 0f;
            _currentSpawnInterval = _initialSpawnInterval;
            _enemiesToSpawn = 1;

            PlayerActionsManager.Get().Reset();
            UpgradeManager.Get().Reset();

            PlayerActionsManager.Get().Reset();
            WeaponManager.Get().Reset();
            HUD.Load();

            AddWorldWalls();
            GenerateWorldObjects();
            _gameObjects.Add(PlayerManager.Get().Player);

            for (var i = 0; i < 1; i++)
                SpawnZombie();
        }

        public void PauseGame()
        {
            if (CurrentGameState == GameState.Paused)
                return;

            ResumeState = CurrentGameState;
            CurrentGameState = GameState.Paused;
        }

        public void ResumeGame()
        {
            CurrentGameState = GameState.Playing;
        }

        public void SetGameState(GameState newState, bool savePrevious = true)
        {
            if (savePrevious)
                PreviousGameState = CurrentGameState;

            CurrentGameState = newState;
        }

        public void Load()
        {
            HUD = new HUD();

            foreach (var gameObject in _gameObjects)
                gameObject.Load();

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
            UpdateGameplayTimers(gameTime);

            if (CurrentGameState == GameState.Tutorial)
            {
                if (InputManager.Get().IsKeyPress(Microsoft.Xna.Framework.Input.Keys.Space))
                {
                    ResetGame();
                    CurrentGameState = GameState.Playing;
                }

                return;
            }

            if (CurrentGameState == GameState.Upgrade)
            {
                UpgradeManager.Get().UpdateButtons(gameTime);
                return;
            }

            if (CurrentGameState == GameState.PassiveUpgrade)
            {
                PassivesManager.Get().UpdateButtons(gameTime);
                return;
            }

            if (CurrentGameState == GameState.Playing)
            {
                CommandManager.Get().Update(gameTime);
                UpdatePhase();
                SpawnEnemies();
                HandleInput();

                foreach (var gameObject in _gameObjects)
                    gameObject.Update(gameTime);

                _camera.Update(PlayerManager.Get().Player.GetPosition(), GetWorldBounds());
                HUD.Update(gameTime);

                CheckCollision();

                foreach (var gameObject in _toBeAdded)
                {
                    gameObject.Load();

                    if (gameObject is Enemy enemy)
                        Enemies.Add(enemy);

                    if (gameObject is Friendly friendly && !Friendlies.Contains(friendly))
                        Friendlies.Add(friendly);

                    _gameObjects.Add(gameObject);
                }

                _toBeAdded.Clear();

                foreach (var gameObject in _toBeRemoved)
                {
                    if (gameObject is Enemy enemy)
                        Enemies.Remove(enemy);

                    if (gameObject is Friendly friendly)
                        Friendlies.Remove(friendly);

                    _gameObjects.Remove(gameObject);
                }

                _toBeRemoved.Clear();
            }
        }

        private void UpdateGameplayTimers(GameTime gameTime)
        {
            if (!IsGameplayRunning)
                return;

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _gameTimeElapsed += deltaTime;
            _spawnTimer += deltaTime;
        }

        private void UpdatePhase()
        {
            var phaseMultiplier = MathF.Pow(1.2f, MathF.Floor(_gameTimeElapsed / 40f));

            _currentSpawnInterval = Settings.GetValue(SettingsConst.SPAWNING.ZOMBIE_SPAWN_INTERVAL) / phaseMultiplier;
            _enemiesToSpawn = Math.Max(1,
                (int)MathF.Round(Settings.GetValue(SettingsConst.SPAWNING.ENEMIES_PER_WAVE) * phaseMultiplier));
            _maxEnemiesOnScreen = Math.Max(1,
                (int)MathF.Round(Settings.GetValue(SettingsConst.SPAWNING.MAX_ENEMIES_ON_SCREEN) * phaseMultiplier));
            _maxBrutesOnScreen = Math.Max(0,
                (int)MathF.Round(Settings.GetValue(SettingsConst.SPAWNING.MAX_BRUTES) * phaseMultiplier));
            _bruteSpawnChance =
                MathHelper.Clamp(Settings.GetValue(SettingsConst.SPAWNING.BRUTE_SPAWN_CHANCE) * phaseMultiplier, 0f,
                    0.6f);
            _babyZombieSpawnChance =
                MathHelper.Clamp(Settings.GetValue(SettingsConst.SPAWNING.BABY_ZOMBIE_SPAWN_CHANCE) * phaseMultiplier,
                    0f, 0.3f);
        }

        private void SpawnEnemies()
        {
            if (_spawnTimer < _currentSpawnInterval)
                return;

            _spawnTimer = 0f;

            int zombieCount = _gameObjects.OfType<Zombie>().Count();
            int bruteCount = _gameObjects.OfType<Brute>().Count();
            int babyZombieCount = _gameObjects.OfType<BabyZombie>().Count();
            int totalEnemies = zombieCount + bruteCount + babyZombieCount;

            if (totalEnemies >= _maxEnemiesOnScreen)
                return;

            var loop = Math.Min(_enemiesToSpawn, _maxEnemiesOnScreen - totalEnemies);

            for (var i = 0; i < loop; i++)
            {
                if (bruteCount < _maxBrutesOnScreen && RNG.NextDouble() < _bruteSpawnChance)
                {
                    SpawnBrute();
                    bruteCount++;
                }
                else if (RNG.NextDouble() < _babyZombieSpawnChance)
                {
                    SpawnBabyZombie();
                    babyZombieCount++;
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

        private void SpawnBabyZombie()
        {
            BabyZombie babyZombie = new BabyZombie();
            Vector2 spawnPos = RandomLocationOutsideView();
            babyZombie.Spawn(spawnPos);
            AddGameObject(babyZombie);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (CurrentGameState)
            {
                case GameState.HealSelection:
                    ScreenManager.Get().DrawHealSelectScreen(spriteBatch);
                    break;

                case GameState.StartScreen:
                    ScreenManager.Get().DrawStartScreen(spriteBatch);
                    break;

                case GameState.Tutorial:
                    ScreenManager.Get().DrawTutorial(spriteBatch);
                    break;

                case GameState.Information:
                    ScreenManager.Get().DrawInformation(spriteBatch);
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

                case GameState.Settings:
                    ScreenManager.Get().DrawSettings(spriteBatch);
                    break;

                case GameState.PassiveUpgrade:
                    DrawGameObjects(spriteBatch, gameTime);

                    spriteBatch.Begin();
                    HUD.Draw(spriteBatch, this);
                    PlayerActionsManager.Get().Draw(spriteBatch);
                    PassivesManager.Get().Draw(spriteBatch, this);
                    spriteBatch.End();
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
                new Stat("Max Health", PlayerManager.Get().Player._maxHealth.ToString()),
                new Stat("Move Speed",
                    (PlayerManager.Get().Player._speed / 10).ToString("0.0", CultureInfo.InvariantCulture)),
                new Stat("Friendlies", _gameObjects.OfType<Friendly>().Count().ToString()),
            };

            foreach (var boost in BoostManager.Get().GetBoosts())
                stats.Add(new Stat("Boost", $"x{boost.GetUnlockedBoostMultiplier():0.00}"));

            if (WeaponManager.Get().IsHealBombUnlocked())
            {
                var healBomb = WeaponManager.Get().HealBomb;
                stats.Add(new Stat("-", "-"));
                stats.Add(new Stat("Health Bomb", "-"));
                stats.Add(new Stat("Radius", healBomb.Radius.ToString()));
                stats.Add(new Stat("Healing", healBomb.HealingAmount.ToString()));
                stats.Add(new Stat("Ticks", healBomb.Ticks.ToString()));
            }

            if (WeaponManager.Get().IsFreezeTrapUnlocked())
            {
                var freezeTrapStats = WeaponManager.Get().GetFreezeTrapStats();
                stats.Add(new Stat("-", "-"));
                stats.Add(new Stat("Freeze Trap", "-"));
                stats.Add(new Stat("Duration", freezeTrapStats.SlowDuration.ToString()));
                stats.Add(new Stat("Slow Factor", freezeTrapStats.SlowFactor.ToString()));
            }

            if (WeaponManager.Get().IsElectricTrapUnlocked())
            {
                var electricTrapStats = WeaponManager.Get().GetElectricTrapStats();
                stats.Add(new Stat("-", "-"));
                stats.Add(new Stat("Electric Trap", "-"));
                stats.Add(new Stat("Damage", electricTrapStats.DamagePerTick.ToString()));
                stats.Add(new Stat("Tick Interval",
                    electricTrapStats.DamageTickInterval.ToString("0.0", CultureInfo.InvariantCulture)));
                stats.Add(new Stat("Stun Duration",
                    electricTrapStats.StunDuration.ToString("0.0", CultureInfo.InvariantCulture)));
                stats.Add(new Stat("Stun Force",
                    electricTrapStats.StunForce.ToString("0.0", CultureInfo.InvariantCulture)));
            }

            if (WeaponManager.Get().IsHealTrapUnlocked())
            {
                var healBombTrapStats = WeaponManager.Get().GetHealBombTrapStats();
                stats.Add(new Stat("-", "-"));
                stats.Add(new Stat("Heal Bomb Trap", "-"));
                stats.Add(new Stat("Healing",
                    healBombTrapStats.HealAmountPerTick.ToString("0.0", CultureInfo.InvariantCulture)));
                stats.Add(new Stat("Tick Interval",
                    healBombTrapStats.HealTickInterval.ToString("0.0", CultureInfo.InvariantCulture)));
                stats.Add(
                    new Stat("Radius", healBombTrapStats.HealRadius.ToString("0.0", CultureInfo.InvariantCulture)));
            }

            if (WeaponManager.Get().IsBombTrapUnlocked())
            {
                var bombTrapStats = WeaponManager.Get().GetBombTrapStats();
                stats.Add(new Stat("-", "-"));
                stats.Add(new Stat("Bomb Trap", "-"));
                stats.Add(new Stat("Activation Delay",
                    bombTrapStats.ActivationDelay.ToString("0.0", CultureInfo.InvariantCulture)));
                stats.Add(new Stat("Damage", bombTrapStats.ExplosionDamage.ToString()));
                stats.Add(new Stat("Radius",
                    bombTrapStats.ExplosionRadius.ToString("0.0", CultureInfo.InvariantCulture)));
                stats.Add(new Stat("Fade Duration",
                    bombTrapStats.ExplosionFadeDuration.ToString("0.0", CultureInfo.InvariantCulture)));
            }

            if (WeaponManager.Get().IsSpikeTrapUnlocked())
            {
                var spikeTrapStats = WeaponManager.Get().GetSpikeTrapStats();
                stats.Add(new Stat("-", "-"));
                stats.Add(new Stat("Spike Trap", "-"));
                stats.Add(new Stat("Damage", spikeTrapStats.DamagePerHit.ToString()));
                stats.Add(new Stat("Damage Interval",
                    spikeTrapStats.DamageInterval.ToString("0.0", CultureInfo.InvariantCulture)));
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

            Vector2 playerPos = PlayerManager.Get().Player.GetPosition();
            float minDistanceFromPlayer = margin;

            for (var i = 0; i < 20; i++)
            {
                var candidate = new Vector2(
                    RNG.Next(safePlayableBounds.Left, safePlayableBounds.Right),
                    RNG.Next(safePlayableBounds.Top, safePlayableBounds.Bottom));

                if (blockedViewBounds.Contains(candidate))
                    continue;

                if (Vector2.Distance(candidate, playerPos) < minDistanceFromPlayer)
                    continue;

                if (!IsAreaFree(candidate, 24f + padding))
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

            foreach (var point in fallbackPoints)
            {
                if (blockedViewBounds.Contains(point))
                    continue;

                if (Vector2.Distance(point, playerPos) < minDistanceFromPlayer)
                    continue;

                if (!IsAreaFree(point, 24f + padding))
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

        private bool IsAreaFree(Vector2 position, float radius)
        {
            var candidate = new Rectangle(
                (int)(position.X - radius),
                (int)(position.Y - radius),
                (int)(radius * 2),
                (int)(radius * 2));

            foreach (var blockedBounds in WorldObstacleBounds)
            {
                if (blockedBounds.Intersects(candidate))
                    return false;
            }

            return true;
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
            AddWorldWall(new Rectangle(
                _playableBounds.Left - WallThickness,
                _playableBounds.Top - WallThickness,
                _playableBounds.Width + WallThickness * 2,
                WallThickness));

            AddWorldWall(new Rectangle(
                _playableBounds.Left - WallThickness,
                _playableBounds.Bottom,
                _playableBounds.Width + WallThickness * 2,
                WallThickness));

            AddWorldWall(new Rectangle(
                _playableBounds.Left - WallThickness,
                _playableBounds.Top,
                WallThickness,
                _playableBounds.Height));

            AddWorldWall(new Rectangle(
                _playableBounds.Right,
                _playableBounds.Top,
                WallThickness,
                _playableBounds.Height));
        }

        private void GenerateWorldObjects()
        {
            WorldObstacleBounds.Clear();
            ProceduralWorldGenerator.Generate(this, AddStaticWorldObject);
            Pathfinder.InitGrid(WorldObstacleBounds);
        }

        private void AddWorldWall(Rectangle bounds)
        {
            var wall = new Wall(bounds);
            wall.Load();
            _gameObjects.Add(wall);
        }

        private void AddStaticWorldObject(GameObject gameObject)
        {
            if (gameObject.collider != null)
                WorldObstacleBounds.Add(gameObject.collider.GetBoundingBox());

            if (Game != null && ContentsManager.Get().GetContent() != null)
                gameObject.Load();

            _gameObjects.Add(gameObject);
        }

        private void DrawGameObjects(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.LinearClamp);

            DrawTiledGameplayBackground(spriteBatch);

            foreach (var gameObject in _gameObjects)
                gameObject.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private void DrawTiledGameplayBackground(SpriteBatch spriteBatch)
        {
            var content = ContentsManager.Get();
            var worldBounds = GetPlayableBounds();

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