using System;
using Microsoft.Xna.Framework;

namespace TheCure.Weapons
{
    public abstract class BaseWeapon : IWeapon
    {
        protected float CoolDown = 0f;
        protected float DamageMultiplier = 1f;

        public abstract float FireRate { get; }

        public bool CanFire => CoolDown <= 0f;

        public void UpdateCoolDown(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (CoolDown > 0f)
            {
                CoolDown -= deltaTime;
            }
        }

        public abstract void Fire(Vector2 position, Vector2 direction);

        public virtual void SetDamageMultiplier(float damageMultiplier)
        {
            DamageMultiplier = Math.Max(0f, damageMultiplier);
        }

        protected void ResetCoolDown()
        {
            if (FireRate > 0)
            {
                CoolDown = FireRate;
            }
        }
    }
}
