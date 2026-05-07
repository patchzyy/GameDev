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
        protected float _maxDuration;
        protected float _elapsedTime;
        protected Color _baseColor = Color.White;
        protected Color _currentColor = Color.White;
        protected const float TrapRadius = 20f;
        protected HashSet<Mob> _hitMobs = new HashSet<Mob>();
        protected bool _isActive = true;

        public Trap(Vector2 position, float duration = 10f)
        {
            _collider = new CircleCollider(position, TrapRadius);
            SetCollider(_collider);
            _maxDuration = duration;
            _elapsedTime = 0f;
            _baseColor = Color.White;
            _currentColor = Color.White;
        }

        public override void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Bullet");
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isActive)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _elapsedTime += deltaTime;

            UpdateTrap(gameTime);

            if (_elapsedTime >= _maxDuration)
            {
                Destroy();
                _isActive = false;
            }

            base.Update(gameTime);
        }

        protected virtual void UpdateTrap(GameTime gameTime) { }

        public override void OnCollision(GameObject other)
        {
            if (!_isActive)
                return;

            if (other is Mob mob && !_hitMobs.Contains(mob))
            {
                _hitMobs.Add(mob);
                OnTrapHit(mob);
            }
        }

        protected abstract void OnTrapHit(Mob mob);

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null || !_isActive)
                return;

            var boundingBox = _collider.GetBoundingBox();

            float alpha = Math.Max(0.2f, 1f - (_elapsedTime / _maxDuration));
            Color drawColor = _currentColor * alpha;

            spriteBatch.Draw(_texture, boundingBox, drawColor);

            base.Draw(gameTime, spriteBatch);
        }

        public Vector2 GetPosition() => _collider.Center;

        public float GetRemainingLifetime() => Math.Max(0f, 1f - (_elapsedTime / _maxDuration));

        public float GetElapsedTime() => _elapsedTime;
    }
}
