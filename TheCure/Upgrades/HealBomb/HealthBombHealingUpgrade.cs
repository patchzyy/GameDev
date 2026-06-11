using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealthBombHealingUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb Healing";
    public string Description { get; } = "Increases the healing amount of the health bomb by 5.";
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; }
    
    public HealthBombHealingUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }
    
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().HealBomb.UpgradeHealingAmount(5f);
    }
}