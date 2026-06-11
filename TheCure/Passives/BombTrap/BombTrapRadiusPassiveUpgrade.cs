using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class BombTrapRadiusPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Radius";
    public string Description { get; } = "Increase bomb trap explosion radius by 20.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapRadiusPassiveUpgrade()
    {
        RequiredUpgrade = new BombTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.BOMB_TRAP.EXPLOSION_RADIUS, 20f);
    }
}

