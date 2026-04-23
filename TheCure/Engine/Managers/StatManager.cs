using System;
using System.Collections.Generic;
using TheCure.Managers;

namespace TheCure.Engine.Managers;

public class StatManager : Manager<StatManager>
{
    public float GetFinalStat(SettingKey<float> statKey)
    {
        float baseValue = Settings.GetValue(statKey);
        float boostMultiplier = BoostManager.Get().GetMultiplier(statKey);

        return baseValue * boostMultiplier;
    }

    public void UpdateFriendlyStats(Friendly friendly)
    {
        friendly.SetWeaponDamage(GetFinalStat(SettingsConst.FRIENDLY.ATTACK_DAMAGE));
        friendly.SetSizeMultiplier(GetFinalStat(SettingsConst.FRIENDLY.SIZE));
    }

    public void UpdateFriendliesStats()
    {
        var friendlies = GameManager.Get().Friendlies;
        foreach (var friendly in friendlies)
        {
            UpdateFriendlyStats(friendly);
        }
    }

    public void UpdateZombieStats(Zombie zombie)
    {
    }

    public void UpdateZombiesStats(List<Zombie> zombies)
    {
        foreach (var zombie in zombies)
        {
            UpdateZombieStats(zombie);
        }
    }
}