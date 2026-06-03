using System;
using System.Collections.Generic;
using TheCure.Managers;
using TheCure.Upgrades;

namespace TheCure.Passives;

public class HealthPassiveUpgrade : Upgrade
{
    public string Name { get; } = "Health Passive";
    public string Description { get; } = "Permanently increase max health by 5";
    public Action Action { get; }
    public bool Unlocked { get; set; }
    public Upgrade RequiredUpgrade { get; set; } = null;
    public UpgradeType Type { get; } = UpgradeType.Passive;
    public bool UnlockedOnce { get; set; } = false;

    public void Unlock(List<Upgrade> unlockedUpgrades)
    {
        var oldHealth = Settings.GetValue(SettingsConst.PLAYER.MAX_HEALTH);
        var newHealth = oldHealth + 5;
        Settings.Save(SettingsConst.PLAYER.MAX_HEALTH, newHealth);
        PlayerManager.Get().Player.ReloadStats();
    }
}