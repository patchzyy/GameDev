using Microsoft.Xna.Framework;
using TheCure.Weapons.Throw;

namespace TheCure.PlayerActions;

public class ThrowMode : PlayerAction
{
    public ThrowMode()
    {
        Cooldown = 0;
    }

    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        gameManager.Player.WeaponsSystem.SetThrowWeapon(ThrowWeapons.HealBomb);
    }
}