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
            GameManager.GetGameManager().Friendlies.Add(this);
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
            var gm = GameManager.GetGameManager();
            Vector2 playerPosition = gm.Player.GetPosition().Center.ToVector2();

            int count = gm.Friendlies.Count;
            if (count == 0) return;

            int index = gm.Friendlies.IndexOf(this);

          
            int ringSize = 6;  
            float baseRadius = 130f;
            float ringSpacing = 80f;

            int ringNumber = index / ringSize;     
            int indexInRing = index % ringSize;

            float orbitRadius = baseRadius + ringNumber * ringSpacing;

            float angleStep = MathHelper.TwoPi / ringSize;
            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            float angle = angleStep * indexInRing + time + _angleOffset;

            Vector2 offset = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * orbitRadius;

            Vector2 targetPos = playerPosition + offset;

            foreach (var other in gm.Friendlies)
            {
                if (other == this) continue;

                float dist = Vector2.Distance(targetPos, other._collider.Center);
                float minDist = _radius * 2;

                if (dist < minDist && dist > 0)
                {
                    Vector2 push = targetPos - other._collider.Center;
                    push.Normalize();
                    targetPos += push * (minDist - dist);
                }
            }

            _collider.Center = Vector2.Lerp(_collider.Center, targetPos, 5f * deltaTime);

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