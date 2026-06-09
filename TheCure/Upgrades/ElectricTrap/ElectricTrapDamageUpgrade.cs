using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class ElectricTrapDamageUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Damage";
    public string Description { get; } = "Increase electric trap damage per tick by 2.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapDamageUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().ElectricTrap.ElectricTrapStats.IncreaseDamage(2);
    }
}

