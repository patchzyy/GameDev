using System;
using System.Collections.Generic;
using TheCure.BaseObjects.Traps;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class ElectricTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap";
    public string Description { get; } = "Place an electric trap that shocks nearby enemies.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Action;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        PlayerActionsManager.Get().AddAction(new Build("Build", TrapType.Electric, 10f));
    }
}