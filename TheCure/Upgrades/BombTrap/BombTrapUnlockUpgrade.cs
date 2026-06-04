using System.Collections.Generic;
using TheCure.BaseObjects.Traps;
using TheCure.PlayerActions;

namespace TheCure.Upgrades;

public class BombTrapUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap";
    public string Description { get; } = "Place a bomb trap that explodes, dealing area damage.";
    public UpgradeType Type { get; } = UpgradeType.Action;
    public Upgrade RequiredUpgrade { get; set; } = null;
    public bool UnlockedOnce { get; set; } = true;
    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        PlayerActionsManager.Get().AddAction(new Build("Build", TrapType.Bomb, 10f));
    }
}