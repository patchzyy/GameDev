using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class FreezeTrap : Trap
    {
        private const float SlowDuration = 2.5f;
        private const float SlowFactor = 0.4f;
        private Dictionary<Mob, float> _slowedMobs = new Dictionary<Mob, float>();
        private Dictionary<Mob, float> _originalSpeeds = new Dictionary<Mob, float>();

        public FreezeTrap(Vector2 position, float duration = 10f) : base(position, duration)
        {
            _baseColor = Color.Cyan;
            _currentColor = Color.Cyan;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            List<Mob> toRemove = new List<Mob>();
            foreach (var kvp in _slowedMobs)
            {
                _slowedMobs[kvp.Key] -= deltaTime;
                if (_slowedMobs[kvp.Key] <= 0f)
                {
                    if (_originalSpeeds.ContainsKey(kvp.Key))
                    {
                        kvp.Key._speed = _originalSpeeds[kvp.Key];
                        _originalSpeeds.Remove(kvp.Key);
                    }
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var mob in toRemove)
            {
                _slowedMobs.Remove(mob);
            }

            float freeze = 0.6f + (0.4f * (float)Math.Sin(_elapsedTime * 5));
            _currentColor = _baseColor * freeze;
        }

        protected override void OnTrapHit(Mob mob)
        {
            if (!_slowedMobs.ContainsKey(mob))
            {
                _originalSpeeds[mob] = mob._speed;
                _slowedMobs[mob] = SlowDuration;
                mob._speed *= SlowFactor;
            }
            else
            {
                _slowedMobs[mob] = SlowDuration;
            }
        }
    }
}
