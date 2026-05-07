using System;
using System.Collections.Generic;
using TheCure.BaseObjects.Traps;

namespace TheCure.Upgrades;

public class BombTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Unlock";
    public string Description { get; } = "Unlock the Bomb Trap for building";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        GameManager.GetGameManager().PlayerInteractionsHud.GetBuild().AddTrapType(typeof(BombTrap));
    }
}