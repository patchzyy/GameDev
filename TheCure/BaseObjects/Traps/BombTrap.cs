using System;
using Microsoft.Xna.Framework;
using TheCure.Enemies;
using TheCure.Entities;

namespace TheCure.BaseObjects.Traps
{
    public class BombTrapStats
    {
        public float ActivationDelay;
        public int ExplosionDamage;
        public float ExplosionRadius;
        public float ExplosionFadeDuration;

        public BombTrapStats()
        {
            ActivationDelay = Settings.GetValue(SettingsConst.BOMB_TRAP.ACTIVATION_DELAY);
            ExplosionDamage = Settings.GetValue(SettingsConst.BOMB_TRAP.EXPLOSION_DAMAGE);
            ExplosionRadius = Settings.GetValue(SettingsConst.BOMB_TRAP.EXPLOSION_RADIUS);
            ExplosionFadeDuration = Settings.GetValue(SettingsConst.BOMB_TRAP.EXPLOSION_FADE_DURATION);
        }

        public void DecreaseActivationDelay(float amount)
        {
            ActivationDelay = Math.Max(0.05f, ActivationDelay - amount);
        }

        public void IncreaseDamage(int amount)
        {
            ExplosionDamage += amount;
        }

        public void IncreaseRadius(float amount)
        {
            ExplosionRadius += amount;
        }

        public void DecreaseFadeDuration(float amount)
        {
            ExplosionFadeDuration = Math.Max(0.01f, ExplosionFadeDuration - amount);
        }
    }

    public class BombTrap : Trap
    {
        private float _activationDelay;
        private int _explosionDamage;
        private float _explosionRadius;
        private float _explosionFadeDuration;

        private bool _activated = false;
        private bool _exploded = false;
        private float _explosionTimer = 0f;

        public BombTrap(BombTrapStats stats, Vector2 position, float duration = 12f) : base(position, duration)
        {
            _activationDelay = stats.ActivationDelay;
            _explosionDamage = stats.ExplosionDamage;
            _explosionRadius = stats.ExplosionRadius;
            _explosionFadeDuration = stats.ExplosionFadeDuration;

            _baseColor = Color.Orange;
            _currentColor = Color.Orange;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!_activated && _elapsedTime >= _activationDelay)
                _activated = true;

            if (_activated && !_exploded)
            {
                float pulse = (float)Math.Sin(_elapsedTime * 4) * 0.3f + 0.7f;
                _currentColor = _baseColor * pulse;
            }

            if (_exploded)
            {
                _explosionTimer += deltaTime;
                float fadeAlpha = 1f - (_explosionTimer / _explosionFadeDuration);
                _currentColor = Color.Red * fadeAlpha;

                if (_explosionTimer >= _explosionFadeDuration)
                {
                    Destroy();
                    _isActive = false;
                }
            }
        }

        protected override void OnTrapHit(LivingEntity target)
        {
            if (target is Enemy && _activated && !_exploded)
                Explode();
        }

        private void Explode()
        {
            _exploded = true;
            _currentColor = Color.Red;
            GameManager gameManager = GameManager.Get();

            if (gameManager.Enemies != null)
            {
                foreach (var enemy in gameManager.Enemies)
                {
                    if (enemy != null && ((CircleCollider)enemy.collider) != null)
                    {
                        Vector2 toEnemy = ((CircleCollider)enemy.collider).Center - ((CircleCollider)collider).Center;
                        float distanceSquared = toEnemy.LengthSquared();
                        float radiusSquared = _explosionRadius * _explosionRadius;

                        if (distanceSquared < radiusSquared)
                        {
                            float distance = (float)Math.Sqrt(distanceSquared);
                            float damageMultiplier = 1f - (distance / _explosionRadius);
                            int damageDealt = (int)(_explosionDamage * damageMultiplier);

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