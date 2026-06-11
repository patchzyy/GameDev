using System.Collections.Generic;

namespace TheCure.Upgrades;

public class HealthBombHealingPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb Healing";
    public string Description { get; } = "Increases the healing amount of the health bomb by 5.";
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; }
    
    public HealthBombHealingPassiveUpgrade()
    {
        RequiredUpgrade = new HealBombTrapUnlockUpgrade();
    }
    
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.HEAL_BOMB.HEALING, 5f);
    }
}