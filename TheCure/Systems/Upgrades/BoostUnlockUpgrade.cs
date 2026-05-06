using System;
using System.Collections.Generic;
using TheCure.PlayerActions;

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
        GameManager.GetGameManager().PlayerInteractionsHud.AddAction(new Boost("Boost"));
    }
}
