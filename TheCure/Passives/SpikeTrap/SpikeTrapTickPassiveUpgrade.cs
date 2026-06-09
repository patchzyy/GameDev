using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class SpikeTrapTickPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Spike Trap Tick Speed";
    public string Description { get; } = "Decrease the damage interval of spike traps by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public SpikeTrapTickPassiveUpgrade()
    {
        RequiredUpgrade = new SpikeTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.SPIKE_TRAP.DAMAGE_INTERVAL, -0.05f);
    }
}

