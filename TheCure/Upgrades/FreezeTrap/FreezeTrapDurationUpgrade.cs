using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class FreezeTrapDurationUpgrade : Upgrade
{
    public string Name { get; } = "Freeze Trap Duration";
    public string Description { get; } = "Increase the duration of freeze traps by 0.2 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public FreezeTrapDurationUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().FreezeTrap.FreezeTrapStats.IncreaseDuration(0.2f);
    }
}