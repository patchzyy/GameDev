using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using TheCure.Mobs;

namespace TheCure
{
    public class Friendly : Mob
    {
        private float _followDistance = 60f;
        private Vector2 _startPosition;
        private float _angleOffset;
        private float _radius = 20f;

        public Friendly() : base("Alien", 80f, 5, 5)
        {
        }

        public Friendly(Vector2 position) : base("Alien", 80f, 5, 5)
        {
            _startPosition = position;
            _angleOffset = (float)(GameManager.GetGameManager().RNG.NextDouble() * MathHelper.TwoPi);
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _collider.Center = _startPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, null, null);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();

            float orbitRadius = 200f;
            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            Vector2 offset = new Vector2(
                (float)Math.Cos(time + _angleOffset),
                (float)Math.Sin(time + _angleOffset)
            ) * orbitRadius;

            Vector2 targetPosition = playerPosition + offset;

            Vector2 direction = targetPosition - _collider.Center;
            float distance = direction.Length();

            if (distance > 1f)
                direction.Normalize();

            Vector2 separation = Vector2.Zero;
            float separationRadius = 40f;

            foreach (var other in GameManager.GetGameManager().Friendlies)
            {
                if (other == this) continue;

                float dist = Vector2.Distance(_collider.Center, other._collider.Center);

                if (dist < separationRadius && dist > 0)
                {
                    Vector2 push = _collider.Center - other._collider.Center;
                    push.Normalize();

                    separation += push * (separationRadius - dist);
                }
            }

            Vector2 velocity = direction + (separation * 0.05f);

            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
                _collider.Center += velocity * (_speed + 20f) * deltaTime;
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