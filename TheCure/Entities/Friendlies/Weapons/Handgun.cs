using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Weapons;

public class Handgun : BaseWeapon
{
    public override float FireRate => 2f;

    public override void Fire(Vector2 position, Vector2 direction)
    {
        Bullet bullet = new Bullet(position, direction, 300f, damage: damage);
        SoundManager.Get().PlayFriendlyShoot();
        GameManager.Get().AddGameObject(bullet);
        ResetCoolDown();
    }
}
