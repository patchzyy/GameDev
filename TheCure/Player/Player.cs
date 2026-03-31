using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Collision;
using TheCure.Weapons;
using System;

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

        internal IWeapon _currentWeapon;
        internal readonly SingleBulletWeapon _bulletWeapon = new SingleBulletWeapon();
        private readonly LaserWeapon _laserWeapon = new LaserWeapon();
        private readonly DoubleBarrelWeapon _doubleBarrelWeapon = new DoubleBarrelWeapon();

        internal float _weaponBuffTimer = 0f;

        internal float _health = 100f;
        public const float MaxHealth = 100f;
        public float Health => _health;

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

            _currentWeapon?.UpdateCoolDown(deltaTime);

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
            if (tmp is Supply)
            {
                Random rng = GameManager.GetGameManager().RNG;

                int weapon = rng.Next(2);

                if (weapon == 0)
                {
                    _currentWeapon = _laserWeapon;
                    System.Diagnostics.Debug.WriteLine("Schip botste met Supply, gewisseld naar LaserWeapon!");
                }
                else
                {
                    _currentWeapon = _doubleBarrelWeapon;
                    System.Diagnostics.Debug.WriteLine("Schip raakte Supply, overgeschakeld op DoubleBarrelWeapon!");
                }

                _weaponBuffTimer = 10f;
            }
        }

        public void Reset()
        {
            _health = MaxHealth;
            _currentWeapon = _bulletWeapon;
            _weaponBuffTimer = 0f;
            _rectangleCollider.shape.Location =
                new Point(GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width / 2,
                    GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height / 2);
            _velocity = Vector2.Zero;
            _rotation = 0f;
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                GameManager.GetGameManager().SetGameState(GameState.GameOver);
            }
        }

        public void Heal(float amount)
        {
            _health = Math.Min(_health + amount, MaxHealth);
        }

        public void SetWeapon(string weaponName)
        {
            switch (weaponName)
            {
                case "Laser":
                    _currentWeapon = _laserWeapon;
                    break;
                case "DoubleBarrel":
                    _currentWeapon = _doubleBarrelWeapon;
                    break;
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