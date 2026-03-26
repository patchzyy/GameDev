using Microsoft.Xna.Framework;

namespace TheCure.Weapons
{
    public class DoubleBarrelWeapon : BaseWeapon
    {
        public override float FireRate => 0.1f;

        public override void Fire(Vector2 position, Vector2 direction, Player owner = null)
        {
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

            Vector2 offset = perpendicular * 10.0f / 2f;

            Vector2 leftBarrelPosition = position + offset;
            Vector2 rightBarrelPosition = position - offset;

            Bullet bullet1 = new Bullet(leftBarrelPosition, direction, 300f);
            Bullet bullet2 = new Bullet(rightBarrelPosition, direction, 300f);

            GameManager gameManager = GameManager.GetGameManager();
                        gameManager.AddGameObject(bullet1);
                        gameManager.AddGameObject(bullet2);

            ResetCoolDown();
        }
    }
}