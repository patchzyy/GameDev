using System;
using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class BombTrap : Trap
    {
        private const float ActivationDelay = 0.7f;
        private const int ExplosionDamage = 25;
        private const float ExplosionRadius = 100f;
        private const float ExplosionFadeDuration = 0.3f;

        private bool _activated = false;
        private bool _exploded = false;
        private float _explosionTimer = 0f;

        public BombTrap(Vector2 position, float duration = 12f) : base(position, duration)
        {
            _baseColor = Color.Orange;
            _currentColor = Color.Orange;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!_activated && _elapsedTime >= ActivationDelay)
            {
                _activated = true;
            }

            if (_activated && !_exploded)
            {
                float pulse = (float)Math.Sin(_elapsedTime * 4) * 0.3f + 0.7f;
                _currentColor = _baseColor * pulse;
            }

            if (_exploded)
            {
                _explosionTimer += deltaTime;
                float fadeAlpha = 1f - (_explosionTimer / ExplosionFadeDuration);
                _currentColor = Color.Red * fadeAlpha;

                if (_explosionTimer >= ExplosionFadeDuration)
                {
                    Destroy();
                    _isActive = false;
                }
            }
        }

        protected override void OnTrapHit(Mob mob)
        {
            if (_activated && !_exploded)
            {
                Explode();
            }
        }

        private void Explode()
        {
            _exploded = true;
            _currentColor = Color.Red;
            GameManager gameManager = GameManager.GetGameManager();

            if (gameManager.Enemies != null)
            {
                foreach (var enemy in gameManager.Enemies)
                {
                    if (enemy != null && enemy._collider != null)
                    {
                        Vector2 toEnemy = enemy._collider.Center - _collider.Center;
                        float distanceSquared = toEnemy.LengthSquared();
                        float radiusSquared = ExplosionRadius * ExplosionRadius;

                        if (distanceSquared < radiusSquared)
                        {
                            float distance = (float)Math.Sqrt(distanceSquared);
                            float damageMultiplier = 1f - (distance / ExplosionRadius);
                            int damageDealt = (int)(ExplosionDamage * damageMultiplier);

                            if (damageDealt > 0)
                            {
                                enemy.LoseHealth(damageDealt);

                                if (distanceSquared > 0)
                                {
                                    toEnemy.Normalize();
                                    float knockBackForce = 250f * damageMultiplier;
                                    enemy.ApplyKnockBack(toEnemy, knockBackForce, 0.5f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
