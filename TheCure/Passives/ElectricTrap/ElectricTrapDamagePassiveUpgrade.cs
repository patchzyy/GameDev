using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class ElectricTrapDamagePassiveUpgrade : Upgrade
{
    public string Name { get; } = "Electric Trap Damage";
    public string Description { get; } = "Increase electric trap damage per tick by 2.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public ElectricTrapDamagePassiveUpgrade()
    {
        RequiredUpgrade = new ElectricTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.ELECTRIC_TRAP.DAMAGE_PER_TICK, 2);
    }
}


