using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class HealthBombTrapHealingPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Heal Bomb Trap Healing";
    public string Description { get; } = "Increase the healing per tick of placed heal bomb traps by 2.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public HealthBombTrapHealingPassiveUpgrade()
    {
        RequiredUpgrade = new HealBombTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.HEAL_BOMB_TRAP.HEALING, 2f);
    }
}