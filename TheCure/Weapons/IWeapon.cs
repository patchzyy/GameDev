using Microsoft.Xna.Framework;

namespace TheCure.Weapons
{
    public interface IWeapon
    {
        float FireRate
        {
            get;
        }
        bool CanFire
        {
            get;
        }

        void UpdateCoolDown(float deltaTime)
        {

        }

        void Fire(Vector2 position, Vector2 direction, Player owner = null)
        {

        }
    }
}