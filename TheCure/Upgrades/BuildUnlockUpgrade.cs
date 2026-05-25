using System;
using System.Collections.Generic;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class BuildUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Build Unlock";
    public string Description { get; } = "Unlock the Build action to place traps";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
       // PlayerActionsManager.Get().AddAction(new Build());
    }
}