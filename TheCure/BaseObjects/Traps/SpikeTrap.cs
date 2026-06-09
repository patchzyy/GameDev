using System;
using Microsoft.Xna.Framework;
using TheCure.Enemies;
using TheCure.Entities;

namespace TheCure.BaseObjects.Traps
{
    public class SpikeTrapStats
    {
        public int DamagePerHit;
        public float DamageInterval;

        public SpikeTrapStats()
        {
            DamagePerHit = Settings.GetValue(SettingsConst.SPIKE_TRAP.DAMAGE_PER_HIT);
            DamageInterval = Settings.GetValue(SettingsConst.SPIKE_TRAP.DAMAGE_INTERVAL);
        }

        public void IncreaseDamage(int amount)
        {
            DamagePerHit += amount;
        }

        public void DecreaseDamageInterval(float amount)
        {
            DamageInterval = Math.Max(0.05f, DamageInterval - amount);
        }
    }

    public class SpikeTrap : Trap
    {
        private int _damagePerHit;
        private float _damageInterval;
        private float _damageTimer = 0f;

        public SpikeTrap(SpikeTrapStats stats, Vector2 position, float duration = 8f) : base(position, duration)
        {
            _damagePerHit = stats.DamagePerHit;
            _damageInterval = stats.DamageInterval;

            _baseColor = Color.Red;
            _currentColor = Color.Red;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float pulseStrength = 0.7f + (0.3f * (float)System.Math.Sin(_elapsedTime * 4));
            _currentColor = _baseColor * pulseStrength;

            _damageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void OnTrapHit(LivingEntity target)
        {
            if (target is Enemy enemy)
            {
                if (_damageTimer <= 0f)
                {
                    enemy.LoseHealth(_damagePerHit);
                    _damageTimer = _damageInterval;
                }
            }
        }
    }
}