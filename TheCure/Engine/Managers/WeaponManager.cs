using TheCure.Managers;
using TheCure.Weapons.Throw;

namespace TheCure.Engine.Managers;

public class WeaponManager : Manager<WeaponManager>
{
    private HealBomb HealBomb { get; set; }
    
    public void Reset()
    {
        HealBomb = null;
    }
    
    public void UnlockHealBomb()
    {
        var healingAmount = Settings.GetValue(SettingsConst.HEAL_BOMB.HEALING);
        var radius = Settings.GetValue(SettingsConst.HEAL_BOMB.RADIUS);
        var ticks = Settings.GetValue(SettingsConst.HEAL_BOMB.TICKS);
        HealBomb = new HealBomb(healingAmount, radius, ticks);
        PlayerActionsManager.Get().AddAction(HealBomb);
    }
    
    public bool IsHealBombUnlocked()
    {
        return HealBomb != null;
    }

    public string HealBombRadius()
    {
        return HealBomb != null ? $"{HealBomb._radius}" : "0";
    }

    public string HealBombHealing()
    {
        return HealBomb != null ? $"{HealBomb._healingAmount}" : "0";
    }
    
    public string HealBombTicks()
    {
        return HealBomb != null ? $"{HealBomb._ticks}" : "0";
    }

    public void UpgradeHealBombHealing(float amount)
    {
        HealBomb.UpgradeHealingAmount(amount);
    }
    
    public void UpgradeHealBombRadius(int amount)
    {
        HealBomb.UpgradeRadius(amount);
    }
    
    public void UpgradeHealBombTicks(int amount)
    {
        HealBomb.UpgradeTicks(amount);
    }
}