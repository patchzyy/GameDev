using Microsoft.Xna.Framework;

namespace TheCure.Weapons
{
    public class LaserWeapon : BaseWeapon
    {
        private readonly float _laserLength = 600f;

        public override float FireRate => 0.10f;

        public override void Fire(Vector2 position, Vector2 direction, GameObject owner = null)
        {
            LinePieceCollider collider = new LinePieceCollider(position, direction, _laserLength);

            Laser laser = new Laser(collider);

            GameManager.GetGameManager().AddGameObject(laser);

            ResetCoolDown();
        }
    }
}