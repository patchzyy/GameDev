using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Collision;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public abstract class Trap : GameObject
    {
        protected CircleCollider _collider;
        protected Texture2D _texture;
        protected float _duration;
        protected float _elapsedTime;
        protected Color _color = Color.White;
        protected const float TrapRadius = 20f;
        protected HashSet<Mob> _hitMobs = new HashSet<Mob>();

        public Trap(Vector2 position, float duration = 10f)
        {
            _collider = new CircleCollider(position, TrapRadius);
            SetCollider(_collider);
            _duration = duration;
            _elapsedTime = 0f;
        }

        public override void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Bullet");
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsedTime >= _duration)
            {
                Destroy();
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Mob mob && !_hitMobs.Contains(mob))
            {
                _hitMobs.Add(mob);
                OnTrapHit(mob);
            }
        }

        protected abstract void OnTrapHit(Mob mob);

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                var boundingBox = _collider.GetBoundingBox();

                float alpha = Math.Max(0.3f, 1f - (_elapsedTime / _duration));
                Color drawColor = _color * alpha;

                spriteBatch.Draw(_texture, boundingBox, drawColor);
            }

            base.Draw(gameTime, spriteBatch);
        }

        public Vector2 GetPosition() => _collider.Center;
    }
}
