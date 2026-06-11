using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Enemies;
using TheCure.Entities;

namespace TheCure.BaseObjects.Traps
{
    public class ElectricTrapStats
    {
        public int DamagePerTick;
        public float DamageTickInterval;
        public float StunDuration;
        public float StunForce;

        public ElectricTrapStats()
        {
            DamagePerTick = Settings.GetValue(SettingsConst.ELECTRIC_TRAP.DAMAGE_PER_TICK);
            DamageTickInterval = Settings.GetValue(SettingsConst.ELECTRIC_TRAP.DAMAGE_TICK_INTERVAL);
            StunDuration = Settings.GetValue(SettingsConst.ELECTRIC_TRAP.STUN_DURATION);
            StunForce = Settings.GetValue(SettingsConst.ELECTRIC_TRAP.STUN_FORCE);
        }

        public void IncreaseDamage(int damageIncrease)
        {
            DamagePerTick += damageIncrease;
        }

        public void DecreaseDamageTickInterval(float intervalDecrease)
        {
            DamageTickInterval = Math.Max(0.1f, DamageTickInterval - intervalDecrease);
        }

        public void IncreaseStunDuration(float durationIncrease)
        {
            StunDuration += durationIncrease;
        }

        public void IncreaseStunForce(float forceIncrease)
        {
            StunForce += forceIncrease;
        }
    }
    
    public class ElectricTrap : Trap
    {
        private int _damagePerTick;
        private float _damageTickInterval;
        private float _stunDuration;
        private float _stunForce;

        private float _tickTimer;
        private HashSet<Enemy> _affectedEnemies = new HashSet<Enemy>();

        public ElectricTrap(ElectricTrapStats stats, Vector2 position, float duration = 10f) : base(position, duration)
        {
            _damagePerTick = stats.DamagePerTick;
            _damageTickInterval = stats.DamageTickInterval;
            _stunDuration = stats.StunDuration;
            _stunForce = stats.StunForce;

            _tickTimer = _damageTickInterval;

            _baseColor = Color.Yellow;
            _currentColor = Color.Yellow;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _tickTimer -= deltaTime;
            if (_tickTimer <= 0f)
            {
                _tickTimer = _damageTickInterval;

                foreach (var enemy in _affectedEnemies)
                {
                    if (enemy != null && _isActive)
                    {
                        enemy.LoseHealth(_damagePerTick);

                        Vector2 pushDirection = ((CircleCollider)enemy.collider).Center - ((CircleCollider)collider).Center;
                        if (pushDirection.LengthSquared() > 0)
                        {
                            pushDirection.Normalize();
                            enemy.ApplyKnockBack(pushDirection, _stunForce, _stunDuration);
                        }
                    }
                }
            }

            float flicker = 0.5f + (0.5f * (float)Math.Sin(_elapsedTime * 8));
            _currentColor = _baseColor * flicker;
        }

        public override void OnCollision(GameObject other)
        {
            if (!_isActive)
                return;

            if (other is Enemy enemy)
            {
                _affectedEnemies.Add(enemy);
            }
        }

        protected override void OnTrapHit(LivingEntity target)
        {
            if (target is Enemy enemy && !_affectedEnemies.Contains(enemy))
            {
                _affectedEnemies.Add(enemy);
                enemy.LoseHealth(_damagePerTick);    

                Vector2 pushDirection = ((CircleCollider)enemy.collider).Center - ((CircleCollider)collider).Center;
                if (pushDirection.LengthSquared() > 0)
                {
                    pushDirection.Normalize();
                    enemy.ApplyKnockBack(pushDirection, _stunForce, _stunDuration);
                }
            }
        }
    }
}
