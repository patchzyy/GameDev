using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Engine.Managers;

namespace TheCure.PlayerActions;

public abstract class Boost : PlayerAction
{
    public bool BoostEnabled { get; set; }
    public float BoostTimer { get; set; }
    public int BoostUpgradeLevel { get; set; }
    public float BoostDuration { get; set; }
    public float BoostBaseMultiplier { get; set; }
    public float BoostUpgradeStep { get; set; }
    public List<SettingKey<float>> BoostSettings { get; } = new();

    public Boost(string iconName) : base(iconName)
    {
        BoostTimer = 0f;
        BoostUpgradeLevel = 0;
        CoolDown = 10f;
    }

    protected override void OnExecute(GameTime gameTime)
    {
        EnableBoost();
    }

    public void EnableBoost()
    {
        BoostEnabled = true;
        BoostTimer = BoostDuration;
        BoostManager.Get().ApplyStatsForBoost(this);
    }

    public void DisableBoost()
    {
        BoostEnabled = false;
    }

    public void Upgrade()
    {
        BoostUpgradeLevel++;
    }

    public float GetBoostMultiplier()
    {
        return BoostTimer > 0f ? GetUnlockedBoostMultiplier() : 1f;
    }

    public float GetUnlockedBoostMultiplier()
    {
        return BoostBaseMultiplier + (BoostUpgradeLevel * BoostUpgradeStep);
    }

    public void Reset()
    {
        BoostEnabled = false;
        BoostTimer = 0f;
        BoostUpgradeLevel = 0;
    }

    public void Update(float deltaTime)
    {
        if (!BoostEnabled) return;

        BoostTimer = Math.Max(0f, BoostTimer - deltaTime);

        if (BoostTimer <= 0f)
        {
            BoostEnabled = false;
        }
    }
}
