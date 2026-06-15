using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class SpikeTrapTickUpgrade : Upgrade
{
    public string Name { get; } = "Spike Trap Tick";
    public string Description { get; } = "Decrease spike trap damage interval by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public SpikeTrapTickUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetSpikeTrapStats();

        if (stats != null)
            stats.DecreaseDamageInterval(0.05f);
    }
}

