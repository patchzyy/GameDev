using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Weapons
{
    public class MovementWeapon : BaseWeapon
    {
        public override float FireRate => 0.2f;

        public MovementWeapon()
        {
            damage = Settings.GetValue(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE);
        }

        public override void Fire(Vector2 position, Vector2 direction)
        {
            Vector2 facingDirection = PlayerManager.Get().Player._facingDirection;
            Bullet bullet = new Bullet(position, facingDirection, 400f, damage, true);
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