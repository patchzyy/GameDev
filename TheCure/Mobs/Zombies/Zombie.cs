using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;

namespace TheCure
{
    internal class Zombie : Mob
    {
        private int _frameWidth;
        private int _frameHeight;

        private int _currentFrame = 0;
        private double _timer = 0;
        private double _fps = 10;

        public Zombie() : base("zombie", 60f, 3, 5)
        {
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _frameWidth = _texture.Width / 5;
            _frameHeight = _texture.Height / 2;
            _collider.Radius = _frameWidth / 4f;

            SetHealthBar(_texture, _maxHealth, _startHealth, BecomeFriendly, null);
            RandomMove();
        }

        public override void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer >= 1.0 / _fps)
            {
                _currentFrame++;
                if (_currentFrame >= 10)
                {
                    _currentFrame = 0;
                }
                _timer = 0;
            }

            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();
            Vector2 direction = playerPosition - _collider.Center;
            float distance = Vector2.Distance(_collider.Center, playerPosition);

            direction.Normalize();
            _collider.Center += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (distance < 40)
            {
                GameManager.GetGameManager().Player.TakeDamage(20f);
                RandomMove();
            }

            base.Update(gameTime);
        }

        private void BecomeFriendly()
        {
            GameManager game = GameManager.GetGameManager();

            game.AddGameObject(new Friendly(_collider.Center));
            game.RemoveGameObject(this);
        }

        public override void OnCollision(GameObject tmp)
        {
            if ((tmp is Bullet || tmp is Laser))
            {
                LoseHealth(1);
            }

            base.OnCollision(tmp);
        }

        public void RandomMove()
        {
            GameManager game = GameManager.GetGameManager();
            _collider.Center = game.RandomLocationOutsideView();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Bereken welke kolom en rij we moeten hebben op basis van _currentFrame
            int column = _currentFrame % 5;
            int row = _currentFrame / 5;

            // De sourceRectangle schuift nu mee met de animatie
            Rectangle sourceRectangle = new Rectangle(
                column * _frameWidth,
                row * _frameHeight,
                _frameWidth,
                _frameHeight
            );

            int displayWidth = _frameWidth / 2;
            int displayHeight = _frameHeight / 2;

            Rectangle destinationRectangle = new Rectangle(
                (int)(_collider.Center.X - displayWidth / 2),
                (int)(_collider.Center.Y - displayHeight / 2),
                displayWidth,
                displayHeight
            );

            spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, Color.White);

            base.Update(gameTime); // Let op: base.Draw gebruiken als je healthbars etc. wilt zien
        }
    }
}