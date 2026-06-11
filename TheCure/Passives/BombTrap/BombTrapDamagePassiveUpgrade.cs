using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class BombTrapDamagePassiveUpgrade : Upgrade
{
    public string Name { get; } = "Bomb Trap Damage";
    public string Description { get; } = "Increase bomb trap explosion damage by 5.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public BombTrapDamagePassiveUpgrade()
    {
        RequiredUpgrade = new BombTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.BOMB_TRAP.EXPLOSION_DAMAGE, 5);
    }
}

