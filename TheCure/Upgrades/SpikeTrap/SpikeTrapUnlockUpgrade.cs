using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class SpikeTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Spike Trap"; 
    public string Description { get; } = "Place a spike trap that damages enemies.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Action;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().UnlockSpikeTrap();
    }
}