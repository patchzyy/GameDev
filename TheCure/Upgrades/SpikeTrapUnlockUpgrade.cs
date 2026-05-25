using System;
using System.Collections.Generic;
using TheCure.BaseObjects.Traps;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class SpikeTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Spike Trap Unlock";
    public string Description { get; } = "Unlock the Spike Trap for building";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        PlayerActionsManager.Get().AddAction(new Build("Build", typeof(SpikeTrap), 10f));
    }
}