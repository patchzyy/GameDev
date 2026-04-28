using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class SpikeTrap : Trap
    {
        private const int DamagePerHit = 15;
        private const float DamageInterval = 0.5f;
        private float _damageTimer = 0f;

        public SpikeTrap(Vector2 position, float duration = 8f) : base(position, duration)
        {
            _baseColor = Color.Red;
            _currentColor = Color.Red;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float pulseStrength = 0.7f + (0.3f * (float)System.Math.Sin(_elapsedTime * 4));
            _currentColor = _baseColor * pulseStrength;

            _damageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void OnTrapHit(Mob mob)
        {
            mob.LoseHealth(DamagePerHit);

            _damageTimer = DamageInterval;
        }
    }
}
