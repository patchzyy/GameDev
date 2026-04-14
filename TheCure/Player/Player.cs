using System;
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

        private Texture2D ship_body;
        private Texture2D base_turret;

        internal readonly RectangleCollider _rectangleCollider;

        internal Vector2 _velocity;
        internal float _rotation;
        private Rectangle _previousBounds;

        internal BaseWeapon _currentWeapon;
        internal readonly SingleBulletWeapon _bulletWeapon = new SingleBulletWeapon();

        internal float _weaponBuffTimer = 0f;

        public WeaponMode CurrentWeaponMode = WeaponMode.Shoot;

        private float _hitTimer = 0f;

        private PlayerAnimationState _currentState;
        private AnimatedSprite _animatedSprite;
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
            SwitchAnimation("Character-Joe-Idle", 6, 6f, true);

            SetHealthBar(content.Load<Texture2D>("Character-Joe-Idle"), (int)MaxHealth, (int)MaxHealth,
                () => GameManager.GetGameManager().SetGameState(GameState.GameOver),
                null, true);

            _rectangleCollider.shape.Size = new Point(
                _animatedSprite.FrameWidth,
                _animatedSprite.FrameHeight
            );

            _rectangleCollider.shape.Location -= new Point(
                _animatedSprite.FrameWidth / 2,
                _animatedSprite.FrameHeight / 2
            );

            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);

            Point mousePosition = inputManager.CurrentMouseState.Position;
            Vector2 worldMousePosition = GameManager.GetGameManager().ScreenToWorld(mousePosition.ToVector2());

            if (inputManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (_currentWeapon != null && _currentWeapon.CanFire)
                {
                    Vector2 aimDirection =
                        LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), worldMousePosition);

                    Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() +
                     aimDirection * 20f;

                    _currentWeapon.Fire(turretExit, aimDirection, this);
                }
            }

            KeyboardState keyState = Keyboard.GetState();
            Vector2 moveDirection = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.W))
            {
                moveDirection.Y = -1;
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                moveDirection.Y = 1;
            }

            if (keyState.IsKeyDown(Keys.A))
            {
                moveDirection.X = -1;
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                moveDirection.X = 1;
            }

            if (moveDirection != Vector2.Zero)
            {
                moveDirection.Normalize();
                _rotation = LinePieceCollider.GetAngle(moveDirection);
            }

            _velocity = moveDirection * MoveSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _animatedSprite.Update(gameTime);

            if (_hitTimer > 0)
            {
                _hitTimer -= deltaTime;

                if (_hitTimer <= 0)
                {
                    SetState(PlayerAnimationState.Idle);
                }
            }

            UpdateState();

            _currentWeapon?.UpdateCoolDown(gameTime);

            if (_weaponBuffTimer > 0)
            {
                _weaponBuffTimer -= deltaTime;

                if (_weaponBuffTimer <= 0)
                {
                    _currentWeapon = _bulletWeapon;
                    System.Diagnostics.Debug.WriteLine("Wapen-buff verlopen. Teruggeschakeld naar BulletWeapon.");
                }
            }

            _previousBounds = _rectangleCollider.shape;
            _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
            _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

            if (_velocity.LengthSquared() > 0.0001f)
                _facingDirection = Vector2.Normalize(_velocity);

            base.Update(gameTime);
        }

        private Texture2D GetCurrentTurretTexture()
        {
            return base_turret;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawShadow(spriteBatch, _rectangleCollider.shape);

            _animatedSprite.Draw(
                spriteBatch,
                _rectangleCollider.shape.Center.ToVector2(),
                Color.White,
                0,
                2f
            );

            base.Draw(gameTime, spriteBatch);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Wall wall)
            {
                // speler netjes terugduwen uit de muur
                wall.ResolveRectangleCollision(_rectangleCollider, _previousBounds, ref _velocity);
            }
        }

        public void Reset()
        {
            _healthBar.ResetHealth();
            _currentWeapon = _bulletWeapon;
            _weaponBuffTimer = 0f;
            _rectangleCollider.shape.Location =
                new Point(GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width / 2,
                    GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height / 2);
            _velocity = Vector2.Zero;
            _rotation = 0f;
        }

        public void SetWeapon(string weaponName)
        {
            switch (weaponName)
            {
                default:
                    _currentWeapon = _bulletWeapon;
                    break;
            }
        }

        public void SetWeaponBuffTimer(float time)
        {
            _weaponBuffTimer = time;
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }

        private void SwitchAnimation(string name, int frames, float fps, bool loop)
        {
            var texture = GameManager.GetGameManager().Content.Load<Texture2D>(name);

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
            if (_currentState == newState)
                return;

            _currentState = newState;

            switch (newState)
            {
                case PlayerAnimationState.Run:
                    SwitchAnimation("Character-Joe-Run", 6, 8f, true);
                    break;

                case PlayerAnimationState.Hit:
                    SwitchAnimation("Character-Joe-Idle-Shot", 4, 10f, false);
                    break;

                default:
                    SwitchAnimation("Character-Joe-Idle", 6, 6f, true);
                    break;
            }
        }

        public void TakeHit()
        {
            SetState(PlayerAnimationState.Hit);
            _hitTimer = 0.4f;
        }

        private void DrawShadow(SpriteBatch spriteBatch, Rectangle destRect)
        {
            Rectangle shadowCore = new Rectangle(
                destRect.Center.X - (int)(destRect.Width * 0.25f),
                destRect.Bottom - (int)(destRect.Height * 0.1f),
                (int)(destRect.Width * 0.5f),
                (int)(destRect.Height * 0.2f)
            );

            spriteBatch.Draw(
                GameManager.GetGameManager().DummyTexture,
                shadowCore,
                Color.Black * 0.2f
            );
        }
    }

    enum PlayerAnimationState
    {
        Idle,
        Run,
        Hit
    }
}