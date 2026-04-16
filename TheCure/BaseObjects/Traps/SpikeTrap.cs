using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class SpikeTrap : Trap
    {
        private const int DamageAmount = 15;

        public SpikeTrap(Vector2 position, float duration = 8f) : base(position, duration)
        {
            _color = Color.Red;
        }

        protected override void OnTrapHit(Mob mob)
        {
            mob.LoseHealth(DamageAmount);
        }
    }
}
