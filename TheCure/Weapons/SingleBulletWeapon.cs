using Microsoft.Xna.Framework;

namespace TheCure.Weapons
{
    public class SingleBulletWeapon : BaseWeapon
    {
        public override float FireRate => 0.2f;

        public override void Fire(Vector2 position, Vector2 direction, Ship owner = null)
        {
            Bullet bullet = new Bullet(position, direction, 300f);

            GameManager.GetGameManager().AddGameObject(bullet);

            ResetCoolDown();
        }
    }
}