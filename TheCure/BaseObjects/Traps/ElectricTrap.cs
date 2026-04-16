using System;
using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class ElectricTrap : Trap
    {
        private const int DamageAmount = 10;
        private const float StunDuration = 2f;
        private const float TickInterval = 0.3f;
        private float _tickTimer = 0f;

        public ElectricTrap(Vector2 position, float duration = 10f) : base(position, duration)
        {
            _color = Color.Yellow;
        }

        public override void Update(GameTime gameTime)
        {
            _tickTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            float flicker = (float)Math.Sin(_elapsedTime * 6) * 0.4f + 0.6f;
            _color = Color.Yellow * flicker;

            base.Update(gameTime);
        }

        protected override void OnTrapHit(Mob mob)
        {
            mob.LoseHealth(DamageAmount);

            Vector2 pushDirection = mob._collider.Center - _collider.Center;
            if (pushDirection.LengthSquared() > 0)
            {
                pushDirection.Normalize();
                mob.ApplyKnockBack(pushDirection, 200f, StunDuration);
            }
        }
    }
}
