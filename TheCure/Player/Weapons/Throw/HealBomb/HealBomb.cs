using Microsoft.Xna.Framework;

namespace TheCure.Weapons.Throw;

public class HealBomb : BaseWeapon
{
    public override float FireRate => 5f;

    public override void Fire(Vector2 position, Vector2 direction)
    {
        HealBombObject healBombObject = new HealBombObject(position, direction, "Bullet");
        GameManager.Get().AddGameObject(healBombObject);

        ResetCoolDown();
    }
}