using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class HealthBombTrapTickPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Heal Bomb Trap Tick Speed";
    public string Description { get; } = "Decrease the tick interval of placed heal bomb traps by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public HealthBombTrapTickPassiveUpgrade()
    {
        RequiredUpgrade = new HealBombTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.HEAL_BOMB_TRAP.TICK_INTERVAL, -0.05f);
    }
}