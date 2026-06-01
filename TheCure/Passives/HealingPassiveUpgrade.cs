using System;
using System.Collections.Generic;
using TheCure.Managers;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class HealingPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Healing Boost";
    public string Description { get; } = "Increase healing power by 5";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = false;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        float oldHealingPower = Settings.GetValue(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE);
        Settings.Save(SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE, oldHealingPower + 5);
        PlayerManager.Get().Player.WeaponsSystem.Reload();
    }
}