using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealthBombTickUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb Tick";
    public string Description { get; } = "Increases the tick times of the health bomb by 1.";
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; }
    
    public HealthBombTickUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }
    
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().UpgradeHealBombTicks(1);
    }
}