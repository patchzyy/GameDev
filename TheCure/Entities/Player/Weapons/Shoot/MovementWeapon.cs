using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Weapons
{
    public class MovementWeapon : BaseWeapon
    {
        public override float FireRate => 0.2f;

        public MovementWeapon()
        {
            this.damage = Settings.GetValue(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE);
        }

        public override void Fire(Vector2 position, Vector2 direction)
        {
            Vector2 facingDirection = PlayerManager.Get().Player._facingDirection;
            Bullet bullet = new Bullet(position, facingDirection, 400f, true, damage: damage);
            SoundManager.Get().PlayPlayerShoot();
            GameManager.Get().AddGameObject(bullet);

            ResetCoolDown();
        }

        public void ResetDamage()
        {
            this.damage = Settings.GetValue(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE);
        }
    }
}