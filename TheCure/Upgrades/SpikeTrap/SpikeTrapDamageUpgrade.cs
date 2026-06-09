using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class SpikeTrapDamageUpgrade : Upgrade
{
    public string Name { get; } = "Spike Trap Damage";
    public string Description { get; } = "Increase spike trap damage per hit by 5.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public SpikeTrapDamageUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetSpikeTrapStats();
        if (stats != null)
        {
            stats.IncreaseDamage(5);
        }
    }
}

