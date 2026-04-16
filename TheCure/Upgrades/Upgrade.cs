using System;
using System.Collections.Generic;

namespace TheCure.Upgrades;

public interface Upgrade
{
    public string Name { get; }
    public string Description { get; }
    public Action Action { get; }

    public bool Unlocked { get; set; }

    public Upgrade RequiredUpgrade { get; set; }

    public bool UnlockedOnce { get; set; }

    public void Unlock(List<Upgrade> unlockedUpgrades);
}