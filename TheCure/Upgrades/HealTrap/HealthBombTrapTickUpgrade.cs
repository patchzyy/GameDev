using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealthBombTrapTickUpgrade : Upgrade
{
    public string Name { get; } = "Heal Bomb Trap Tick Speed";
    public string Description { get; } = "Decrease the heal tick interval of heal bomb traps by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public HealthBombTrapTickUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetHealBombTrapStats();

        if (stats != null)
            stats.DecreaseTickInterval(0.05f);
    }
}