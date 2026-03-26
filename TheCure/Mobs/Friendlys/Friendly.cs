using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;

namespace TheCure
{
    internal class Friendly : Mob
    {
        private float _followDistance = 60f;
        private Vector2 _startPosition;
        private int _frameWidth;
        private int _frameHeight;

        public Friendly() : base("friendly", 60f, 3, 5)
        {
        }

        public Friendly(Vector2 position) : base("friendly", 60f, 3, 5)
        {
            _startPosition = position;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            _frameWidth = _texture.Width / 5;
            _frameHeight = _texture.Height / 2;

            _collider.Radius = _frameWidth / 4f;
            _collider.Center = _startPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, null, null);
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();
            Vector2 direction = playerPosition - _collider.Center;
            float distance = Vector2.Distance(_collider.Center, playerPosition);

            if (distance > _followDistance)
            {
                direction.Normalize();
                _collider.Center += direction * (_speed + 20f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Zombie zombie)
            {
                zombie.LoseHealth(1);

                Vector2 pushDir = zombie._collider.Center - _collider.Center;

                pushDir.Normalize();
                zombie._collider.Center += pushDir * 5;
            }

            base.OnCollision(tmp);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int frameWidth = _texture.Width / 5;
            int frameHeight = _texture.Height / 2;

            int displayWidth = frameWidth / 2;
            int displayHeight = frameHeight / 2;

            Rectangle sourceRectangle = new Rectangle(0, 0, frameWidth, frameHeight);

            Rectangle destinationRectangle = new Rectangle(
                (int)(_collider.Center.X - (displayWidth / 2)),
                (int)(_collider.Center.Y - (displayHeight / 2)),
                displayWidth,
                displayHeight
            );

            Color tint = Color.LightBlue;
            spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, tint);

            string text = "Friendly";
            Vector2 textSize = _font.MeasureString(text);
            Vector2 textPos = new Vector2(
                _collider.Center.X - (textSize.X / 2),
                _collider.Center.Y - (displayHeight / 2) - 25
            );

            spriteBatch.DrawString(_font, text, textPos, Color.LimeGreen);

            base.Draw(gameTime, spriteBatch);
        }
    }
}