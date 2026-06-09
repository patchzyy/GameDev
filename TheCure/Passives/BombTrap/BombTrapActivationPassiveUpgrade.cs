using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class BombTrapActivationPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Activation";
    public string Description { get; } = "Decrease bomb trap activation delay by 0.1 seconds.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapActivationPassiveUpgrade()
    {
        RequiredUpgrade = new BombTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.BOMB_TRAP.ACTIVATION_DELAY, -0.1f);
    }
}

