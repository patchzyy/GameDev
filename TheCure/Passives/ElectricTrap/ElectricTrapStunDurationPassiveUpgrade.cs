using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class ElectricTrapStunDurationPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Stun Duration";
    public string Description { get; } = "Increase the stun duration of electric traps by 0.1 seconds.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapStunDurationPassiveUpgrade()
    {
        RequiredUpgrade = new ElectricTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.ELECTRIC_TRAP.STUN_DURATION, 0.1f);
    }
}


