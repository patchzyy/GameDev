using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Enemies;
using TheCure.Entities;

namespace TheCure.BaseObjects.Traps
{
    public class FreezeTrapStats
    {
        public float SlowDuration;
        public float SlowFactor;

        public FreezeTrapStats()
        {
            SlowDuration = Settings.GetValue(SettingsConst.FREEZE_TRAP.DURATION);
            SlowFactor = Settings.GetValue(SettingsConst.FREEZE_TRAP.SLOW_FACTOR);
        }

        public void IncreaseDuration(float durationIncrease)
        {
            SlowDuration += durationIncrease;
        }

        public void IncreaseSlowFactor(float factorIncrease)
        {
            SlowFactor += factorIncrease;
        }
    }

    public class FreezeTrap : Trap
    {
        private float SlowDuration { get; set; }
        private float SlowFactor { get; set; }
        private Dictionary<Enemy, float> _slowedEnemies = new Dictionary<Enemy, float>();
        private Dictionary<Enemy, float> _originalSpeeds = new Dictionary<Enemy, float>();

        public FreezeTrap(FreezeTrapStats stats, Vector2 position, float duration = 10f) : base(position, duration)
        {
            SlowDuration = stats.SlowDuration;
            SlowFactor = stats.SlowFactor;
            _baseColor = Color.Cyan;
            _currentColor = Color.Cyan;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            List<Enemy> toRemove = new List<Enemy>();
            foreach (var kvp in _slowedEnemies)
            {
                _slowedEnemies[kvp.Key] -= deltaTime;
                if (_slowedEnemies[kvp.Key] <= 0f)
                {
                    if (_originalSpeeds.ContainsKey(kvp.Key))
                    {
                        kvp.Key._speed = _originalSpeeds[kvp.Key];
                        _originalSpeeds.Remove(kvp.Key);
                    }

                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var enemy in toRemove)
            {
                _slowedEnemies.Remove(enemy);
            }

            float freeze = 0.6f + (0.4f * (float)Math.Sin(_elapsedTime * 5));
            _currentColor = _baseColor * freeze;
        }

        protected override void OnTrapHit(LivingEntity target)
        {
            if (target is Enemy enemy && !_slowedEnemies.ContainsKey(enemy))
            {
                _originalSpeeds[enemy] = enemy._speed;
                _slowedEnemies[enemy] = SlowDuration;
                enemy._speed *= SlowFactor;
            }
            else
            {
                _slowedEnemies[target as Enemy] = SlowDuration;
            }
        }
    }
}