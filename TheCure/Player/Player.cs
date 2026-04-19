using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Collision;
using TheCure.Weapons;

namespace TheCure
{
    public class Player : GameObject
    {
        public float MoveSpeed;
        public float MaxHealth;

        internal readonly RectangleCollider _rectangleCollider;
        internal Vector2 _velocity;
        internal float _rotation;

        private Rectangle _previousBounds;

        // ===== WEAPONS =====
        public WeaponsSystem WeaponsSystem = new WeaponsSystem();

        internal BaseWeapon _currentWeapon;
        internal readonly SingleBulletWeapon _bulletWeapon = new SingleBulletWeapon();
        internal float _weaponBuffTimer = 0f;

        // ===== CHARACTER JOE =====
        private PlayerAnimationState _currentState;
        private AnimatedSprite _animatedSprite;
        private float _hitTimer = 0f;

        private Vector2 _facingDirection = Vector2.UnitX;

        public Player(Point Position)
        {
            MoveSpeed = Settings.GetValue(SettingsConst.PLAYER.MOVE_SPEED);
            MaxHealth = Settings.GetValue(SettingsConst.PLAYER.MAX_HEALTH);

            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
            SetCollider(_rectangleCollider);

            _velocity = Vector2.Zero;
            _rotation = 0f;

            _currentWeapon = _bulletWeapon;
            _previousBounds = _rectangleCollider.shape;
        }

        public override void Load(ContentManager content)
        {
            // ===== CHARACTER JOE ONLY =====
            SwitchAnimation("Character-Joe-Idle", 5, 1f, true);

            SetHealthBar(
                content.Load<Texture2D>("Character-Joe-Idle"),
                (int)MaxHealth,
                (int)MaxHealth,
                () => GameManager.GetGameManager().SetGameState(GameState.GameOver),
                null,
                true
            );

            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);

            // ===== SHOOTING =====
            Point mousePosition = inputManager.CurrentMouseState.Position;
            Vector2 worldMouse = GameManager.GetGameManager()
                .ScreenToWorld(mousePosition.ToVector2());

            if (inputManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (_currentWeapon != null && _currentWeapon.CanFire)
                {
                    Vector2 aimDirection =
                        LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), worldMouse);

                    Vector2 spawnPos =
                        _rectangleCollider.shape.Center.ToVector2() +
                        aimDirection * 20f;

                    _currentWeapon.Fire(spawnPos, aimDirection);
                }
            }

            // ===== MOVEMENT =====
            KeyboardState keyState = Keyboard.GetState();
            Vector2 moveDirection = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.W)) moveDirection.Y = -1;
            if (keyState.IsKeyDown(Keys.S)) moveDirection.Y = 1;
            if (keyState.IsKeyDown(Keys.A)) moveDirection.X = -1;
            if (keyState.IsKeyDown(Keys.D)) moveDirection.X = 1;

            if (moveDirection != Vector2.Zero)
            {
                moveDirection.Normalize();
                _rotation = LinePieceCollider.GetAngle(moveDirection);
                _facingDirection = moveDirection;
            }

            _velocity = moveDirection * MoveSpeed;

            var dash = GameManager.GetGameManager().HUD?.GetDash();
            if (dash != null && dash.IsDashing)
            {
                _velocity = Vector2.Zero;
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            WeaponsSystem.Update(gameTime);
            _currentWeapon?.UpdateCoolDown(gameTime);

            // ===== WEAPON BUFF =====
            if (_weaponBuffTimer > 0)
            {
                _weaponBuffTimer -= deltaTime;
                if (_weaponBuffTimer <= 0)
                {
                    _currentWeapon = _bulletWeapon;
                }
            }

            // ===== HIT TIMER =====
            if (_hitTimer > 0)
            {
                _hitTimer -= deltaTime;
                if (_hitTimer <= 0)
                {
                    SetState(PlayerAnimationState.Idle);
                }
            }

            UpdateState();

            _previousBounds = _rectangleCollider.shape;

            _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
            _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

            _animatedSprite?.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects effects = SpriteEffects.None;

            if (_facingDirection.X < 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            _animatedSprite?.Draw(
                spriteBatch,
                _rectangleCollider.shape.Center.ToVector2(),
                Color.White,
                0f,
                2f,
                effects
            );
            base.Draw(gameTime, spriteBatch);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Wall wall)
            {
                wall.ResolveRectangleCollision(_rectangleCollider, _previousBounds, ref _velocity);
            }
        }

        // ===== ANIMATION SYSTEM (CHARACTER JOE) =====

        private void SwitchAnimation(string name, int frames, float fps, bool loop)
        {
            var texture = GameManager.GetGameManager()._content.Load<Texture2D>(name);
            int frameWidth = texture.Width / frames;

            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop);
        }

        private void UpdateState()
        {
            if (_currentState == PlayerAnimationState.Hit)
                return;

            if (_velocity.LengthSquared() > 0.01f)
                SetState(PlayerAnimationState.Run);
            else
                SetState(PlayerAnimationState.Idle);
        }

        private void SetState(PlayerAnimationState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;

            switch (newState)
            {
                case PlayerAnimationState.Run:
                    SwitchAnimation("Character-Joe-Run", 8, 8f, true);
                    break;

                case PlayerAnimationState.Hit:
                    SwitchAnimation("Character-Joe-Idle-Shot", 6, 10f, false);
                    break;

                default:
                    SwitchAnimation("Character-Joe-Idle", 5, 1f, true);
                    break;
            }
        }

        public void TakeHit()
        {
            SetState(PlayerAnimationState.Hit);
            _hitTimer = 0.4f;
        }

        public void Reset()
        {
            _healthBar.ResetHealth();
            _currentWeapon = _bulletWeapon;
            _weaponBuffTimer = 0f;

            _rectangleCollider.shape.Location =
                new Point(
                    GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width / 2,
                    GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height / 2
                );

            _velocity = Vector2.Zero;
            _rotation = 0f;
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }
    }

    enum PlayerAnimationState
    {
        Idle,
        Run,
        Hit
    }
}