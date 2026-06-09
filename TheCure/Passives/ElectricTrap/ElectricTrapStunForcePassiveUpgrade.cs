using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class ElectricTrapStunForcePassiveUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Stun Force";
    public string Description { get; } = "Increase the stun knockback force of electric traps by 50.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapStunForcePassiveUpgrade()
    {
        RequiredUpgrade = new ElectricTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.ELECTRIC_TRAP.STUN_FORCE, 50f);
    }
}


