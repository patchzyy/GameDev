using System.Collections.Generic;

namespace TheCure.Upgrades;

public class HealthBombTickPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb Ticks";
    public string Description { get; } = "Increases the number of ticks the health bomb lasts by 1.";
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; }
    
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.HEAL_BOMB.TICKS, 1);
    }
}