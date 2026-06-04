using System.Collections.Generic;

namespace TheCure.Upgrades;

public class FreezeTrapDurationPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Freeze Trap Duration";
    public string Description { get; } = "Increase the duration of freeze traps by 0.2 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;
    public FreezeTrapDurationPassiveUpgrade()
    {
        RequiredUpgrade = new FreezeTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.FREEZE_TRAP.DURATION, 0.2f);
    }
}