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

    public void UnlockElectricTrap()
    {
        ElectricTrap = new Build("Build", TrapType.Electric, 5f);
        PlayerActionsManager.Get().AddAction(ElectricTrap);
    }

    public ElectricTrapStats GetElectricTrapStats()
    {
        if (ElectricTrap == null)
        {
            return null;
        }

        return ElectricTrap.ElectricTrapStats;
    }

    public void UnlockHealTrap()
    {
        HealTrap = new Build("Build", TrapType.HealBomb, 10f);
        PlayerActionsManager.Get().AddAction(HealTrap);
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
        BombTrapBuild = new Build("Build", TrapType.Bomb, 12f);
        PlayerActionsManager.Get().AddAction(BombTrapBuild);
    }

    public void UnlockSpikeTrap()
    {
        SpikeTrapBuild = new Build("Build", TrapType.Spike, 8f);
        PlayerActionsManager.Get().AddAction(SpikeTrapBuild);
    }

    public BombTrapStats GetBombTrapStats()
    {
        if (BombTrapBuild == null)
        {
            return null;
        }

        return BombTrapBuild.BombTrapStats;
    }

    public SpikeTrapStats GetSpikeTrapStats()
    {
        if (SpikeTrapBuild == null)
        {
            return null;
        }

        return SpikeTrapBuild.SpikeTrapStats;
    }
}