using Microsoft.Xna.Framework;
using TheCure.Weapons;

namespace TheCure.PlayerActions;

public class ShootMode : PlayerAction
{
    public ShootMode()
    {
        Cooldown = 0;
    }

    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        gameManager.Player.WeaponsSystem.SetShootWeapon(ShootWeapons.SingleBullet);
    }
}