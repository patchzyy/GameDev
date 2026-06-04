using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class FreezeTrapSlowUpgrade : Upgrade
{
    public string Name { get; } = "Freeze Trap Slow Factor";
    public string Description { get; } = "Increase the slow factor of freeze traps by 0.1.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public FreezeTrapSlowUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().FreezeTrap.FreezeTrapStats.IncreaseSlowFactor(0.1f);
    }
}