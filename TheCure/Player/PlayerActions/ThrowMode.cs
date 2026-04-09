using Microsoft.Xna.Framework;
using TheCure.Weapons;

namespace TheCure.PlayerActions;

public class ThrowMode : PlayerAction
{
    public ThrowMode()
    {
        Cooldown = 0;
    }

    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        gameManager.Player.CurrentWeaponMode = WeaponMode.Throw;
    }
}