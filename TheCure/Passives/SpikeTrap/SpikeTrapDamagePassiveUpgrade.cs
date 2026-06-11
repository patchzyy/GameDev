using System.Collections.Generic;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class SpikeTrapDamagePassiveUpgrade : Upgrade
{
    public string Name { get; } = "Spike Trap Damage";
    public string Description { get; } = "Increase spike trap damage per hit by 5.";
    public Upgrade RequiredUpgrade { get; set; }
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = true;

    public SpikeTrapDamagePassiveUpgrade()
    {
        RequiredUpgrade = new SpikeTrapUnlockUpgrade();
    }

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        Settings.UpgradeValue(SettingsConst.SPIKE_TRAP.DAMAGE_PER_HIT, 5);
    }
}

