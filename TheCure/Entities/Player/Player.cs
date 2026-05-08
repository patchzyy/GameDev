using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Collision;
using TheCure.Entities;
using TheCure.Managers;
using TheCure.Weapons;

namespace TheCure
{
    public class Player : LivingEntity
    {
        internal float _rotation;
        private Vector2 _previousCenter;

        public WeaponsSystem WeaponsSystem = new WeaponsSystem();

        internal BaseWeapon _currentWeapon;
        internal readonly SingleBulletWeapon _bulletWeapon = new SingleBulletWeapon();
        internal float _weaponBuffTimer = 0f;

        private PlayerAnimationState _currentState;
        private float _hitTimer = 0f;

        public Player(Vector2 position) : base(
            textureName: "Character-Joe-Idle",
            speed: Settings.GetValue(SettingsConst.PLAYER.MOVE_SPEED),
            startHealth: Settings.GetValue(SettingsConst.PLAYER.MAX_HEALTH),
            maxHealth: Settings.GetValue(SettingsConst.PLAYER.MAX_HEALTH),
            frameCount: 5,
            frameRate: 1f,
            isLooping: true,
            scale: 2f
        )
        {
            collider = new CircleCollider(position, 16f);
            SetCollider(collider);

            _velocity = Vector2.Zero;
            _rotation = 0f;

            _currentWeapon = _bulletWeapon;
        }

        public override void Load()
        {
            SwitchAnimation("Character-Joe-Idle", 5, 1f, true);

            var content = ContentsManager.Get().GetContent();
            var idleTexture = content.Load<Texture2D>("Character-Joe-Idle");

            SetHealthBar(
                idleTexture,
                _maxHealth,
                _maxHealth,
                () => GameManager.Get().SetGameState(GameState.GameOver),
                null,
                true
            );
            SyncHealthBarPosition();

            base.Load();
        }

        public override void HandleInput()
        {
            base.HandleInput();

            var inputManager = InputManager.Get();

            if (inputManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                WeaponsSystem.Fire();
            }

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

            var dash = PlayerActionsManager.Get().GetDash();
            if (dash == null || !dash.IsDashing)
                _velocity = moveDirection * _speed;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            WeaponsSystem.Update(gameTime);
            _currentWeapon?.UpdateCoolDown(gameTime);

            if (_weaponBuffTimer > 0)
            {
                _weaponBuffTimer -= deltaTime;
                if (_weaponBuffTimer <= 0)
                {
                    _currentWeapon = _bulletWeapon;
                }
            }

            if (_hitTimer > 0)
            {
                _hitTimer -= deltaTime;
                if (_hitTimer <= 0)
                {
                    SetState(PlayerAnimationState.Idle);
                }
            }

            UpdateState();

            _previousCenter = ((CircleCollider)collider).Center;

            ((CircleCollider)collider).Center += _velocity * deltaTime;

            //_animatedSprite?.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects effects = SpriteEffects.None;

            if (_facingDirection.X < 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Color tint = _isFlashing ? _flashColor : Color.White;
            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Wall wall)
            {
                Vector2 collisionNormal = wall.ResolveCircleCollision((CircleCollider)collider, _previousCenter);
                if (collisionNormal != Vector2.Zero)
                {
                    float velocityIntoWall = Vector2.Dot(_velocity, collisionNormal);
                    if (velocityIntoWall < 0f)
                    {
                        _velocity -= collisionNormal * velocityIntoWall;
                    }
                }
            }

            base.OnCollision(tmp);
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
            if (_currentState == newState)
                return;

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
            _healthBar?.ResetHealth();
            _currentWeapon = _bulletWeapon;
            _weaponBuffTimer = 0f;

            ((CircleCollider)collider).Center =
                new Vector2(
                    GameManager.Get().Game.GraphicsDevice.Viewport.Width / 2,
                    GameManager.Get().Game.GraphicsDevice.Viewport.Height / 2
                );

            _velocity = Vector2.Zero;
            _rotation = 0f;
        }

        public Vector2 GetPosition()
        {
            return ((CircleCollider)collider).Center;
        }
    }

    enum PlayerAnimationState
    {
        Idle,
        Run,
        Hit
    }
}
