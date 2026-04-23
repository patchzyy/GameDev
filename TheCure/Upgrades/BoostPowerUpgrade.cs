using System;
using System.Collections.Generic;
using TheCure.Boosts;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class BoostPowerUpgrade : Upgrade
{
    public string Name { get; } = "Boost Power";
    public string Description { get; } = "Increase Boost strength by +0.05x";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; } = false;

    public BoostPowerUpgrade(Upgrade requiredUpgrade)
    {
        RequiredUpgrade = requiredUpgrade;
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        if (RequiredUpgrade != null && !unlockedUpgrades.Contains(RequiredUpgrade))
        {
            return;
        }
        
        BoostManager.Get().Upgrade<FriendlyBoost>();
    }
}