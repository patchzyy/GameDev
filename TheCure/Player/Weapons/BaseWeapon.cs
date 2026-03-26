using Microsoft.Xna.Framework;

namespace TheCure.Weapons
{
    public abstract class BaseWeapon : IWeapon
    {
        protected float CoolDown = 0f;

        public abstract float FireRate
        {
            get;
        }

        public bool CanFire => CoolDown <= 0f;

        public virtual void UpdateCoolDown(float time)
        {
            if (CoolDown > 0f)
            {
                CoolDown -= time;
            }
        }

        public abstract void Fire(Vector2 position, Vector2 direction, Player owner = null);

        protected virtual void ResetCoolDown()
        {
            if (FireRate > 0)
            {
                CoolDown = FireRate;
            }
        }
    }
}