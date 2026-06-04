using System;
using System.Collections.Generic;

namespace TheCure.Upgrades;

public interface Upgrade
{
    public string Name { get; }
    public string Description { get; }
    public UpgradeType Type { get; }
    public Upgrade RequiredUpgrade { get; set; }
    public bool UnlockedOnce { get; set; }

    public void Unlock(List<Upgrade> unlockedUpgrades);
}