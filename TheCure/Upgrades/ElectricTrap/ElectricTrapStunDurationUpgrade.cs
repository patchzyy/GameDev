using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class ElectricTrapStunDurationUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Stun Duration";
    public string Description { get; } = "Increase the stun duration of electric traps by 0.1 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapStunDurationUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().ElectricTrap.ElectricTrapStats.IncreaseStunDuration(0.1f);
    }
}

