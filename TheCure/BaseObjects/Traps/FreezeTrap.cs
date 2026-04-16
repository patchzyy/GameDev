using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class FreezeTrap : Trap
    {
        private const float SlowDuration = 3f;
        private const float SlowFactor = 0.3f;

        public FreezeTrap(Vector2 position, float duration = 10f) : base(position, duration)
        {
            _color = Color.Cyan;
        }

        protected override void OnTrapHit(Mob mob)
        {
            mob._speed *= SlowFactor;
        }
    }
}
