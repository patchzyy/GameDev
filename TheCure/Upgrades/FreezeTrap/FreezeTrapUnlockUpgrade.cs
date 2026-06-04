using System;
using System.Collections.Generic;
using TheCure.BaseObjects.Traps;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class FreezeTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Freeze Trap";
    public string Description { get; } = "Place a freeze trap that slows down nearby enemies.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Action;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        PlayerActionsManager.Get().AddAction(new Build("Build", typeof(FreezeTrap), 10f));
    }
}