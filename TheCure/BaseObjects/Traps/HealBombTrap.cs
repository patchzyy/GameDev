using System;
using Microsoft.Xna.Framework;
using TheCure.Entities;

namespace TheCure.BaseObjects.Traps
{
    public class HealBombStats
    {
        public float HealAmountPerTick;
        public float HealTickInterval;
        public float HealRadius;

        public HealBombStats()
        {
            HealAmountPerTick = Settings.GetValue(SettingsConst.HEAL_BOMB_TRAP.HEALING);
            HealTickInterval = Settings.GetValue(SettingsConst.HEAL_BOMB_TRAP.TICK_INTERVAL);
            HealRadius = Settings.GetValue(SettingsConst.HEAL_BOMB_TRAP.RADIUS);
        }

        public void IncreaseHealing(float amount)
        {
            HealAmountPerTick += amount;
        }

        public void DecreaseTickInterval(float amount)
        {
            HealTickInterval = Math.Max(0.05f, HealTickInterval - amount);
        }

        public void IncreaseRadius(float amount)
        {
            HealRadius += amount;
        }
    }

    public class HealBombTrap : Trap
    {
        private float _healAmountPerTick;
        private float _healTickInterval;
        private float _healRadius;

        private float _healTickTimer;

        public HealBombTrap(HealBombStats stats, Vector2 position, float duration = 15f) : base(position, duration)
        {
            _healAmountPerTick = stats.HealAmountPerTick;
            _healTickInterval = stats.HealTickInterval;
            _healRadius = stats.HealRadius;

            _healTickTimer = _healTickInterval;

            _baseColor = Color.LimeGreen;
            _currentColor = Color.LimeGreen;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _healTickTimer -= deltaTime;
            if (_healTickTimer <= 0f)
            {
                _healTickTimer = _healTickInterval;

                GameManager gameManager = GameManager.Get();

                if (gameManager.Friendlies != null)
                {
                    foreach (var friendly in gameManager.Friendlies)
                    {
                        if (friendly != null && ((CircleCollider)friendly.collider != null))
                        {
                            Vector2 toFriendly = ((CircleCollider)friendly.collider).Center - (_collider).Center;
                            float distance = toFriendly.Length();

                            if (distance < _healRadius)
                                friendly.GainHealth((int)_healAmountPerTick);
                        }
                    }
                }

                if (gameManager.Enemies != null)
                {
                    foreach (var enemy in gameManager.Enemies)
                    {
                        if (enemy != null && ((CircleCollider)enemy.collider != null))
                        {
                            Vector2 toEnemy = ((CircleCollider)enemy.collider).Center - (_collider).Center;
                            float distance = toEnemy.Length();

                            if (distance < _healRadius)
                                enemy.GainHealth((int)_healAmountPerTick);
                        }
                    }
                }
            }

            float pulse = 0.6f + (0.4f * (float)Math.Sin(_elapsedTime * 3));
            _currentColor = _baseColor * pulse;
        }

        protected override void OnTrapHit(LivingEntity target)
        {
            if (target is Friendly || target is Zombie)
            {
                target.GainHealth((int)(_healAmountPerTick * 2));
            }
        }
    }
}
