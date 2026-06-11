using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class ElectricTrapTickUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Tick Speed";
    public string Description { get; } = "Decrease the damage tick interval of electric traps by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapTickUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().ElectricTrap.ElectricTrapStats.DecreaseDamageTickInterval(0.05f);
    }
}

