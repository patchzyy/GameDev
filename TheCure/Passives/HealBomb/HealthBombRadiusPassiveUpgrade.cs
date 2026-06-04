using System.Collections.Generic;

namespace TheCure.Upgrades;

public class HealthBombRadiusPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb Radius";
    public string Description { get; } = "Increases the radius of the health bomb by 5.";
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; }
    
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.HEAL_BOMB.RADIUS, 5);
    }
}