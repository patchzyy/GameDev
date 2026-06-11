using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Weapons
{
    public class InstantWeapon : BaseWeapon
    {
        public override float FireRate => 10f;

        public InstantWeapon()
        {
            this.damage = Settings.GetValue(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE);
        }

        public override void Fire(Vector2 position, Vector2 direction)
        {
            Bullet bullet = new Bullet(position, direction, 300f, true, 10000f);
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