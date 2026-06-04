using System;
using System.Collections.Generic;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class CommandUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Command";
    public string Description { get; } = "Command friendlies to attack around your cursor";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Action;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        PlayerActionsManager.Get().AddAction(new Command("Command"));
    }
}