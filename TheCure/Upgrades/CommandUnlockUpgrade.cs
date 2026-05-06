using System;
using System.Collections.Generic;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class CommandUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Command Unlock";
    public string Description { get; } = "Command friendlies to attack around your cursor";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        GameManager.GetGameManager().PlayerInteractionsHud.AddAction(new Command("Command"));
    }
}
