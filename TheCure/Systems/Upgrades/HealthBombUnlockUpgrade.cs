using System;
using System.Collections.Generic;
using TheCure.PlayerActions;
using TheCure.Weapons.Throw;

namespace TheCure.Upgrades;

public class HealthBombUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb Unlock";
    public string Description { get; } = "Unlock the Health Bomb action";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        GameManager.GetGameManager().PlayerInteractionsHud.AddAction(new Throw("Throw", ThrowWeapons.HealBomb));
    }
}