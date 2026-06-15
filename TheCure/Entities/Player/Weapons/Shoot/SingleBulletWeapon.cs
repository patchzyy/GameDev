using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Weapons
{
    public class SingleBulletWeapon : BaseWeapon
    {
        public override float FireRate => 0.2f;

        public SingleBulletWeapon()
        {
            damage = Settings.GetValue(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE);
        }

        public override void Fire(Vector2 position, Vector2 direction)
        {
            Bullet bullet = new Bullet(position, direction, 300f, damage, true);
            SoundManager.Get().PlayPlayerShoot();
            GameManager.Get().AddGameObject(bullet);

            ResetCoolDown();
        }

        public void ResetDamage()
        {
            damage = Settings.GetValue(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE);
        }
    }
}