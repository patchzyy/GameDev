using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class ElectricTrap : Trap
    {
        private const int DamagePerTick = 8;
        private const float DamageTickInterval = 0.3f;
        private const float StunDuration = 0.8f;
        private const float StunForce = 300f;

        private float _tickTimer = DamageTickInterval;
        private HashSet<Mob> _affectedMobs = new HashSet<Mob>();

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

                foreach (var mob in _affectedMobs)
                {
                    if (mob != null && _isActive)
                    {
                        mob.LoseHealth(DamagePerTick);

                        Vector2 pushDirection = mob._collider.Center - _collider.Center;
                        if (pushDirection.LengthSquared() > 0)
                        {
                            pushDirection.Normalize();
                            mob.ApplyKnockBack(pushDirection, StunForce, StunDuration);
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

            if (other is Mob mob)
            {
                _affectedMobs.Add(mob);
            }
        }

        protected override void OnTrapHit(Mob mob)
        {
            if (!_affectedMobs.Contains(mob))
            {
                _affectedMobs.Add(mob);
                mob.LoseHealth(DamagePerTick);

                Vector2 pushDirection = mob._collider.Center - _collider.Center;
                if (pushDirection.LengthSquared() > 0)
                {
                    pushDirection.Normalize();
                    mob.ApplyKnockBack(pushDirection, StunForce, StunDuration);
                }
            }
        }
    }
}
