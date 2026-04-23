using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.PlayerActions;

namespace TheCure.Engine.Managers;

public class BoostManager : Manager<BoostManager>
{
    public List<Boost> _boosts = new();

    public void AddBoost(Boost boost)
    {
        _boosts.Add(boost);
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var boost in _boosts)
        {
            float before = boost.GetBoostMultiplier();
            bool wasEnabled = boost.BoostEnabled;

            boost.Update(deltaTime);

            float after = boost.GetBoostMultiplier();
            bool changed =
                wasEnabled != boost.BoostEnabled ||
                Math.Abs(before - after) > 0.0001f;
            
            
            if (changed)
            {
                ApplyStatsForBoost(boost);
            }
        }
    }

    public void Upgrade<T>() where T : Boost
    {
        foreach (var boost in _boosts)
        {   
            if (boost is not T) continue;
            boost.Upgrade();
        }
    }

    public void Reset()
    {
        foreach (var boost in _boosts)
        {
            float before = boost.GetBoostMultiplier();
            bool wasEnabled = boost.BoostEnabled;

            boost.Reset();

            float after = boost.GetBoostMultiplier();
            bool changed =
                wasEnabled != boost.BoostEnabled ||
                Math.Abs(before - after) > 0.0001f;

            if (changed)
            {
                ApplyStatsForBoost(boost);
            }
        }
    }

    public float GetMultiplier(SettingKey<float> statKey)
    {
        float multiplier = 1f;

        foreach (var boost in _boosts)
        {
            if (boost.BoostSetting == statKey)
            {
                multiplier *= boost.GetBoostMultiplier();
            }
        }

        return multiplier;
    }

    public void ApplyStatsForBoost(Boost boost)
    {
        if (boost.BoostSetting == SettingsConst.FRIENDLY.ATTACK_DAMAGE)
        {
            StatManager.Get().UpdateFriendliesStats();
        }
    }
}