using System;
using System.Collections.Generic;
using TheCure.Boosts;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class BoostUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Boost Unlock";
    public string Description { get; } = "Unlock the Boost action for your friendlies";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var boost = new FriendlyBoost("Boost");
        PlayerActionsManager.Get().AddAction(boost);
        BoostManager.Get().AddBoost(boost);
    }
}