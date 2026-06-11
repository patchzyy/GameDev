using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class BombTrapFadeUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Fade";
    public string Description { get; } = "Decrease explosion fade duration by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapFadeUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetBombTrapStats();
        if (stats != null)
        {
            stats.DecreaseFadeDuration(0.05f);
        }
    }
}

