using System.Collections.Generic;
using TheCure.Engine.Managers;

namespace TheCure.Upgrades;

public class HealthBombUnlockUpgrade : Upgrade
{
    public string Name { get; } = "Health Bomb";
    public string Description { get; } = "Throw a health bomb to heal everyone in an area.";
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Action;
    public bool UnlockedOnce { get; set; } = true;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        WeaponManager.Get().UnlockHealBomb();
    }
}