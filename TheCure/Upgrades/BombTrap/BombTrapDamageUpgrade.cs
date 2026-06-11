using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class BombTrapDamageUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Damage";
    public string Description { get; } = "Increase bomb trap explosion damage by 5.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapDamageUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var stats = WeaponManager.Get().GetBombTrapStats();
        if (stats != null)
        {
            stats.IncreaseDamage(5);
        }
    }
}

