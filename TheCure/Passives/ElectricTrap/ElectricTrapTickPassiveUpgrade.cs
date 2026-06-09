using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class ElectricTrapTickPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Tick Speed";
    public string Description { get; } = "Decrease the damage tick interval of electric traps by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapTickPassiveUpgrade()
    {
        RequiredUpgrade = new ElectricTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.ELECTRIC_TRAP.DAMAGE_TICK_INTERVAL, -0.05f);
    }
}


