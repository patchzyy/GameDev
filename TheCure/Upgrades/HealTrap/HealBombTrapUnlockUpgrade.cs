using System.Collections.Generic;
using TheCure.BaseObjects.Traps;
using TheCure.PlayerActions;

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
        PlayerActionsManager.Get().AddAction(new Build("Build", typeof(HealBombTrap), 10f));
    }
}