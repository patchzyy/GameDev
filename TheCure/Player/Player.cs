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
        
        public WeaponsSystem WeaponsSystem = new WeaponsSystem();

        public Player(Point Position)
        {
            MoveSpeed = Settings.GetValue(SettingsConst.PLAYER.MOVE_SPEED);
            MaxHealth = Settings.GetValue(SettingsConst.PLAYER.MAX_HEALTH);

            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));

            SetCollider(_rectangleCollider);

            _velocity = Vector2.Zero;
            _rotation = 0f;
            _previousBounds = _rectangleCollider.shape;
        }

        public override void Load(ContentManager content)
        {
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");

            SetHealthBar(ship_body, (int)MaxHealth, (int)MaxHealth,
                () => GameManager.GetGameManager().SetGameState(GameState.GameOver),
                null, true);

            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);

            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);

            if (inputManager.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                WeaponsSystem.Fire(inputManager);
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

            var hud = GameManager.GetGameManager().HUD;
            var dash = hud?.GetDash();

            if (dash == null || !dash.IsDashing)
            {
                _velocity = moveDirection * MoveSpeed;
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            WeaponsSystem.Update(gameTime);

            _previousBounds = _rectangleCollider.shape;
            _rectangleCollider.shape.X += (int)(_velocity.X * deltaTime);
            _rectangleCollider.shape.Y += (int)(_velocity.Y * deltaTime);

            base.Update(gameTime);
        }

        private Texture2D GetCurrentTurretTexture()
        {
            return base_turret;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 bodyOrigin = ship_body.Bounds.Size.ToVector2() / 2f;

            Rectangle shadowCore = new Rectangle(
                _rectangleCollider.shape.Center.X - (int)(bodyOrigin.X * 0.55f),
                _rectangleCollider.shape.Center.Y + (int)(bodyOrigin.Y * 0.38f),
                (int)(bodyOrigin.X * 1.1f),
                (int)(bodyOrigin.Y * 0.18f)
            );

            Rectangle shadowSoft = new Rectangle(
                _rectangleCollider.shape.Center.X - (int)(bodyOrigin.X * 0.65f),
                _rectangleCollider.shape.Center.Y + (int)(bodyOrigin.Y * 0.42f),
                (int)(bodyOrigin.X * 1.3f),
                (int)(bodyOrigin.Y * 0.12f)
            );

            spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, shadowSoft, Color.Black * 0.08f);
            spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, shadowCore, Color.Black * 0.16f);

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
            if (tmp is Wall wall)
            {
                // speler netjes terugduwen uit de muur
                wall.ResolveRectangleCollision(_rectangleCollider, _previousBounds, ref _velocity);
            }
        }

        public override void LoseHealth(int amount)
        {
            var hud = GameManager.GetGameManager().HUD;
            var dash = hud?.GetDash();

            if (dash != null && dash.IsDashing)
            {
                System.Diagnostics.Debug.WriteLine("Player is protected by dash - no damage taken!");
                return;
            }

            base.LoseHealth(amount);
        }

        public void Reset()
        {
            _healthBar.ResetHealth();
            WeaponsSystem.SetShootWeapon(ShootWeapons.SingleBullet);
            _rectangleCollider.shape.Location =
                new Point(GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width / 2,
                    GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Height / 2);
            _velocity = Vector2.Zero;
            _rotation = 0f;
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }
    }
}