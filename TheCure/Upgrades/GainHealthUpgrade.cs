using System;
using System.Collections.Generic;
using TheCure.Managers;

namespace TheCure.Upgrades;

public class GainHealthUpgrade : Upgrade
{
    public string Name { get; } = "Gain Health";
    public string Description { get; } = "Gain 1 health";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = false;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        if (unlockedUpgrades.Contains(RequiredUpgrade) || RequiredUpgrade == null)
        {
            PlayerManager.Get().Player.GainHealth(1);
        }
    }
}