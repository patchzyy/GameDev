using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class BombTrapActivationUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Activation";
    public string Description { get; } = "Decrease bomb trap activation delay by 0.1 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapActivationUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetBombTrapStats();

        if (stats != null)
            stats.DecreaseActivationDelay(0.1f);
    }
}

