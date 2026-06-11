using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealBombTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Heal Bomb Trap";
    public string Description { get; } = "Place a heal bomb trap that heals your friendly units.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Action;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().UnlockHealTrap();
    }
}