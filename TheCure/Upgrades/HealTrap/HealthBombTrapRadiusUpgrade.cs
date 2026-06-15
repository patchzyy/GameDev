using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealthBombTrapRadiusUpgrade : Upgrade
{
    public string Name { get; } = "Heal Bomb Trap Radius";
    public string Description { get; } = "Increase the radius of heal bomb traps by 15.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public HealthBombTrapRadiusUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetHealBombTrapStats();

        if (stats != null)
            stats.IncreaseRadius(15f);
    }
}