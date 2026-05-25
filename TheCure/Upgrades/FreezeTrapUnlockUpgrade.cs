using System;
using System.Collections.Generic;
using TheCure.BaseObjects.Traps;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class FreezeTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Freeze Trap Unlock";
    public string Description { get; } = "Unlock the Freeze Trap for building";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        PlayerActionsManager.Get().AddAction(new Build("Build", typeof(FreezeTrap), 10f));
    }
}