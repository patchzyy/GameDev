using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class BombTrapFadePassiveUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Explosion Fade";
    public string Description { get; } = "Decrease explosion fade duration by 0.05 seconds.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapFadePassiveUpgrade()
    {
        RequiredUpgrade = new BombTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.BOMB_TRAP.EXPLOSION_FADE_DURATION, -0.05f);
    }
}

