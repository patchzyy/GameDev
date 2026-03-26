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

        public Friendly() : base("Alien", 80f, 5, 5)
        {
        }

        public Friendly(Vector2 position) : base("Alien", 80f, 5, 5)
        {
            _startPosition = position;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
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
            Color tint = Color.LightBlue;
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), tint);


            string text = "Friendly";
            Vector2 textSize = _font.MeasureString(text);
            Vector2 textPos = new Vector2(_collider.Center.X - (textSize.X / 2),
                _collider.Center.Y - (_texture.Height / 2) - 20);
            spriteBatch.DrawString(_font, text, textPos, Color.LimeGreen);

            base.Draw(gameTime, spriteBatch);
        }
    }
}