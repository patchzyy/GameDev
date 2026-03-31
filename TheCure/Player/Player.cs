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
        public const float MoveSpeed = 300f;

        private Texture2D ship_body;
        private Texture2D base_turret;
        private Texture2D laser_turret;
        private Texture2D double_turret;

        internal readonly RectangleCollider _rectangleCollider;

        internal Vector2 _velocity;
        internal float _rotation;

        internal BaseWeapon _currentWeapon;
        internal readonly SingleBulletWeapon _bulletWeapon = new SingleBulletWeapon();

        internal float _weaponBuffTimer = 0f;

        public const float MaxHealth = 10;

        public Player(Point Position)
        {
            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));

            SetCollider(_rectangleCollider);

            _velocity = Vector2.Zero;
            _rotation = 0f;
            _currentWeapon = _bulletWeapon;
        }

        public override void Load(ContentManager content)
        {
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");

            SetHealthBar(ship_body, (int)MaxHealth, (int)MaxHealth,
                () => GameManager.GetGameManager().SetGameState(GameState.GameOver),
                null, true);
            try
            {
                double_turret = content.Load<Texture2D>("double_turret");
            }
            catch (ContentLoadException)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Waarschuwing: Kon textuur 'double_turret' niet laden. 'base_turret' wordt gebruikt als fallback.");
                double_turret = base_turret;
            }

            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);

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

                    Texture2D currentTurretTexture = GetCurrentTurretTexture();
                    Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() +
                                         aimDirection * (currentTurretTexture.Height / 2f);

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

            _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
            _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

            base.Update(gameTime);
        }

        private Texture2D GetCurrentTurretTexture()
        {
            if (_currentWeapon is LaserWeapon)
            {
                return laser_turret;
            }
            else if (_currentWeapon is DoubleBarrelWeapon)
            {
                return double_turret;
            }
            else
            {
                return base_turret;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 bodyOrigin = ship_body.Bounds.Size.ToVector2() / 2f;
            spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, _rotation,
                bodyOrigin, 1f, SpriteEffects.None, 0);

            Texture2D texture = GetCurrentTurretTexture();

            Point screenMouse = Mouse.GetState().Position;

            Vector2 mouse = GameManager.GetGameManager().ScreenToWorld(screenMouse.ToVector2());
            Vector2 direction = LinePieceCollider.GetDirection(GetPosition().Center.ToVector2(), mouse);

            float angle = LinePieceCollider.GetAngle(direction);

            Vector2 position = _rectangleCollider.shape.Center.ToVector2();
            Vector2 origin = texture.Bounds.Size.ToVector2() / 2f;

            spriteBatch.Draw(texture, position, null, Color.White, angle, origin, 1f, SpriteEffects.None, 0);

            base.Draw(gameTime, spriteBatch);
        }

        public override void OnCollision(GameObject tmp)
        {
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
    }
}