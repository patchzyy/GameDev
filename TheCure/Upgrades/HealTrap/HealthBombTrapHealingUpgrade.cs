using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealthBombTrapHealingUpgrade : Upgrade
{
    public string Name { get; } = "Heal Bomb Trap Healing";
    public string Description { get; } = "Increase heal bomb trap healing per tick by 2.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public HealthBombTrapHealingUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetHealBombTrapStats();

        if (stats != null)
            stats.IncreaseHealing(2f);
    }
}