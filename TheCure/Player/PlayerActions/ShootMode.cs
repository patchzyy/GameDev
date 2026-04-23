using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.Weapons;

namespace TheCure.PlayerActions;

public class ShootMode : PlayerAction
{
    public ShootMode(string iconName) : base(iconName)
    {
        CoolDown = 0;
    }

    protected override void OnExecute(GameTime gameTime)
    {
        PlayerManager.Get().Player.WeaponsSystem.SetShootWeapon(ShootWeapons.SingleBullet);
    }
}