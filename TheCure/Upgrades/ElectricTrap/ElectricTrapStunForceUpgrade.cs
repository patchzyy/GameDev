using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class ElectricTrapStunForceUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Stun Force";
    public string Description { get; } = "Increase the stun knockback force of electric traps by 0.5.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapStunForceUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().ElectricTrap.ElectricTrapStats.IncreaseStunForce(0.5f);
    }
}

