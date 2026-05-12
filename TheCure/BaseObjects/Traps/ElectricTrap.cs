using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Enemies;
using TheCure.Entities;

namespace TheCure.BaseObjects.Traps
{
    public class ElectricTrap : Trap
    {
        private const int DamagePerTick = 8;
        private const float DamageTickInterval = 0.3f;
        private const float StunDuration = 0.8f;
        private const float StunForce = 300f;

        private float _tickTimer = DamageTickInterval;
        private HashSet<Enemy> _affectedEnemies = new HashSet<Enemy>();

        public ElectricTrap(Vector2 position, float duration = 10f) : base(position, duration)
        {
            _baseColor = Color.Yellow;
            _currentColor = Color.Yellow;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _tickTimer -= deltaTime;
            if (_tickTimer <= 0f)
            {
                _tickTimer = DamageTickInterval;

                foreach (var enemy in _affectedEnemies)
                {
                    if (enemy != null && _isActive)
                    {
                        enemy.LoseHealth(DamagePerTick);

                        Vector2 pushDirection = ((CircleCollider)enemy.collider).Center - ((CircleCollider)collider).Center;
                        if (pushDirection.LengthSquared() > 0)
                        {
                            pushDirection.Normalize();
                            enemy.ApplyKnockBack(pushDirection, StunForce, StunDuration);
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
                enemy.LoseHealth(DamagePerTick);

                Vector2 pushDirection = ((CircleCollider)enemy.collider).Center - ((CircleCollider)collider).Center;
                if (pushDirection.LengthSquared() > 0)
                {
                    pushDirection.Normalize();
                    enemy.ApplyKnockBack(pushDirection, StunForce, StunDuration);
                }
            }
        }
    }
}
