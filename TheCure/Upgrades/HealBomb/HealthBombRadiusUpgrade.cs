using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealthBombRadiusUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb Radius";
    public string Description { get; } = "Increases the radius of the health bomb by 10.";   
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; }
    
    public HealthBombRadiusUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }
    
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().HealBomb.UpgradeRadius(10);
    }
}