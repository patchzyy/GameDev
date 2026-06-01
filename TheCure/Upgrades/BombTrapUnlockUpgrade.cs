using System;
using System.Collections.Generic;
using TheCure.BaseObjects.Traps;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class BombTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Unlock";
    public string Description { get; } = "Unlock the Bomb Trap for building";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Action;
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        PlayerActionsManager.Get().AddAction(new Build("Build", typeof(BombTrap), 10f));
    }
}