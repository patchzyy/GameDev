using System.Collections.Generic;

namespace TheCure.Upgrades;

public class FreezeTrapSlowPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Freeze Trap Slow Factor";
    public string Description { get; } = "Increase the slow factor of freeze traps by 0.1.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.FREEZE_TRAP.SLOW_FACTOR, 0.1f);
    }
}