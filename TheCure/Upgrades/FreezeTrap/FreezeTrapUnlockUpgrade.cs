using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class FreezeTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Freeze Trap";
    public string Description { get; } = "Place a freeze trap that slows down nearby enemies.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Action;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().UnlockFreezeTrap();
    }
}