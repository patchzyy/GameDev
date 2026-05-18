using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Weapons
{
    public class SingleBulletWeapon : BaseWeapon
    {
        public override float FireRate => 0.2f;

        public override void Fire(Vector2 position, Vector2 direction)
        {
            Bullet bullet = new Bullet(position, direction, 300f, true);
            SoundManager.Get().PlayPlayerShoot();
            GameManager.Get().AddGameObject(bullet);

            ResetCoolDown();
        }
    }
}