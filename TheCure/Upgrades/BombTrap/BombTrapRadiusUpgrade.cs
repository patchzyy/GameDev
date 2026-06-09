using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class BombTrapRadiusUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Radius";
    public string Description { get; } = "Increase bomb trap explosion radius by 20.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapRadiusUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetBombTrapStats();
        if (stats != null)
        {
            stats.IncreaseRadius(20f);
        }
    }
}

