using TheCure.BaseObjects.Traps;
using TheCure.Managers;
using TheCure.PlayerActions;
using TheCure.Weapons.Throw;

namespace TheCure.Engine.Managers;

public class WeaponManager : Manager<WeaponManager>
{
    public HealBomb HealBomb { get; private set; }
    public Build FreezeTrap { get; private set; }

    public void Reset()
    {
        HealBomb = null;
        FreezeTrap = null;
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
    
    public void UnlockFreezeTrap()
    {
        FreezeTrap = new Build("Build", TrapType.Freeze, 5f);
        PlayerActionsManager.Get().AddAction(FreezeTrap);
    }

    public bool IsFreezeTrapUnlocked()
    {
        return FreezeTrap != null;
    }
    
    public FreezeTrapStats GetFreezeTrapStats()
    {
        if (FreezeTrap == null)
        {
            return null;
        }
        
        return FreezeTrap.FreezeTrapStats;
    }
}