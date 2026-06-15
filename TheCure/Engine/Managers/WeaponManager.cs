using TheCure.BaseObjects.Traps;
using TheCure.Managers;
using TheCure.PlayerActions;
using TheCure.Weapons.Throw;

namespace TheCure.Engine.Managers;

public class WeaponManager : Manager<WeaponManager>
{
    public HealBomb HealBomb { get; private set; }
    public Build FreezeTrap { get; private set; }

    public Build ElectricTrap { get; private set; }
    public Build HealTrap { get; private set; }
    public Build BombTrapBuild { get; private set; }
    public Build SpikeTrapBuild { get; private set; }

    public void Reset()
    {
        HealBomb = null;
        FreezeTrap = null;
        ElectricTrap = null;
        HealTrap = null;
        BombTrapBuild = null;
        SpikeTrapBuild = null;
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
        FreezeTrap = new Build("FreezeTrap", TrapType.Freeze, 5f);
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

    public void UnlockElectricTrap()
    {
        ElectricTrap = new Build("ElectricTrap", TrapType.Electric, 5f);
        PlayerActionsManager.Get().AddAction(ElectricTrap);
    }

    public bool IsElectricTrapUnlocked()
    {
        return ElectricTrap != null;
    }

    public ElectricTrapStats GetElectricTrapStats()
    {
        if (ElectricTrap == null)
            return null;

        return ElectricTrap.ElectricTrapStats;
    }

    public void UnlockHealTrap()
    {
        HealTrap = new Build("HealTrap", TrapType.HealBomb, 10f);
        PlayerActionsManager.Get().AddAction(HealTrap);
    }

    public bool IsHealTrapUnlocked()
    {
        return HealTrap != null;
    }

    public HealBombStats GetHealBombTrapStats()
    {
        if (HealTrap == null)
        {
            return null;
        }

        return HealTrap.HealBombStats;
    }

    public void UnlockBombTrap()
    {
        BombTrapBuild = new Build("BombTrap", TrapType.Bomb, 12f);
        PlayerActionsManager.Get().AddAction(BombTrapBuild);
    }

    public bool IsBombTrapUnlocked()
    {
        return BombTrapBuild != null;
    }

    public BombTrapStats GetBombTrapStats()
    {
        if (BombTrapBuild == null)
            return null;

        return BombTrapBuild.BombTrapStats;
    }

    public void UnlockSpikeTrap()
    {
        SpikeTrapBuild = new Build("SpikeTrap", TrapType.Spike, 8f);
        PlayerActionsManager.Get().AddAction(SpikeTrapBuild);
    }

    public bool IsSpikeTrapUnlocked()
    {
        return SpikeTrapBuild != null;
    }

    public SpikeTrapStats GetSpikeTrapStats()
    {
        if (SpikeTrapBuild == null)
            return null;

        return SpikeTrapBuild.SpikeTrapStats;
    }
}