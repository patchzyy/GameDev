using System;
using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class BombTrap : Trap
    {
        private const float ActivationDelay = 0.5f;
        private const int ExplosionDamage = 25;
        private const float ExplosionRadius = 80f;
        private bool _activated = false;
        private bool _exploded = false;

        public BombTrap(Vector2 position, float duration = 12f) : base(position, duration)
        {
            _color = Color.Orange;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_activated && _elapsedTime >= ActivationDelay)
            {
                _activated = true;
            }

            if (_activated && !_exploded)
            {
                float pulse = (float)Math.Sin(_elapsedTime * 4) * 0.3f + 0.7f;
                _color = Color.Orange * pulse;
            }
        }

        protected override void OnTrapHit(Mob mob)
        {
            if (_activated && !_exploded)
            {
                Explode(mob);
            }
        }

        private void Explode(Mob mob)
        {
            _exploded = true;
            _color = Color.Red;

            GameManager gameManager = GameManager.GetGameManager();

            foreach (var enemy in gameManager.Enemies)
            {
                if (enemy != null)
                {
                    Vector2 toEnemy = enemy._collider.Center - _collider.Center;
                    float distance = toEnemy.Length();

                    if (distance < ExplosionRadius)
                    {
                        enemy.LoseHealth(ExplosionDamage);
                    }
                }
            }

            _duration = _elapsedTime + 0.3f;
        }
    }
}
