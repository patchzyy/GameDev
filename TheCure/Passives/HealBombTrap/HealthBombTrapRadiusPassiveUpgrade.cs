using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class HealthBombTrapRadiusPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Heal Bomb Trap Radius";
    public string Description { get; } = "Increase the radius of placed heal bomb traps by 15.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public HealthBombTrapRadiusPassiveUpgrade()
    {
        RequiredUpgrade = new HealBombTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.HEAL_BOMB_TRAP.RADIUS, 15);
    }
}