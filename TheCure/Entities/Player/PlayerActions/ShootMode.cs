using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.Weapons;

namespace TheCure.PlayerActions;

public class ShootMode : PlayerAction
{
    public ShootMode(string iconName) : base(iconName)
    {
        CoolDown = PlayerManager.Get().Player.WeaponsSystem.GetFireRate();
    }

    public override void Load()
    {
        CoolDown = PlayerManager.Get().Player.WeaponsSystem.GetFireRate();
        var weaponMode = PlayerManager.Get().Player.WeaponsSystem.CurrentWeaponMode;

        switch (weaponMode)
        {
            case WeaponMode.Throw:
                _iconName = "Throw";
                break;
            case WeaponMode.Shoot:
                _iconName = "Shoot";
                break;
        }

        base.Load();
    }

    public override float GetRemainingCoolDown()
    {
        return PlayerManager.Get().Player.WeaponsSystem.CurrentWeapon.RemainingCoolDown;
    }

    public override void Execute(GameTime gameTime)
    {
        // Shooting is handled in the WeaponsSystem.
        // Do not start a separate PlayerAction cooldown here.
    }

    protected override void OnExecute(GameTime gameTime)
    {
        // Shooting is handled in the WeaponsSystem.
    }
}