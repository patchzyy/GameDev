using System;
using System.Collections.Generic;
using TheCure.Managers;

namespace TheCure.Upgrades;

public class GainHealthUpgrade : Upgrade
{
    public string Name { get; } = "Gain Health";
    public string Description { get; } = "Restore 10 health.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = false;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        if (unlockedUpgrades.Contains(RequiredUpgrade) || RequiredUpgrade == null)
        {
            PlayerManager.Get().Player.GainHealth(10);
        }
    }
}