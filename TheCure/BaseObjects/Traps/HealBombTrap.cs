using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Mobs;

namespace TheCure.BaseObjects.Traps
{
    public class HealBombTrap : Trap
    {
        private const int HealAmountPerTick = 5;
        private const float HealTickInterval = 0.4f;
        private const float HealRadius = 120f;
        private const int ConversionDamage = 10;

        private float _healTickTimer = HealTickInterval;
        private HashSet<Friendly> _healedFriendlies = new HashSet<Friendly>();

        public HealBombTrap(Vector2 position, float duration = 15f) : base(position, duration)
        {
            _baseColor = Color.LimeGreen;
            _currentColor = Color.LimeGreen;
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _healTickTimer -= deltaTime;
            if (_healTickTimer <= 0f)
            {
                _healTickTimer = HealTickInterval;

                GameManager gameManager = GameManager.GetGameManager();

                if (gameManager.Friendlies != null)
                {
                    foreach (var friendly in gameManager.Friendlies)
                    {
                        if (friendly != null && friendly._collider != null)
                        {
                            Vector2 toFriendly = friendly._collider.Center - _collider.Center;
                            float distance = toFriendly.Length();

                            if (distance < HealRadius)
                            {
                                friendly.GainHealth(HealAmountPerTick);
                            }
                        }
                    }
                }

                if (gameManager.Enemies != null)
                {
                    foreach (var enemy in gameManager.Enemies)
                    {
                        if (enemy != null && enemy._collider != null)
                        {
                            Vector2 toEnemy = enemy._collider.Center - _collider.Center;
                            float distance = toEnemy.Length();

                            if (distance < HealRadius)
                            {
                                enemy.LoseHealth(ConversionDamage);
                            }
                        }
                    }
                }
            }

            float pulse = 0.6f + (0.4f * (float)Math.Sin(_elapsedTime * 3));
            _currentColor = _baseColor * pulse;
        }

        protected override void OnTrapHit(Mob mob)
        {
            if (mob is Friendly friendly)
            {
                friendly.GainHealth(HealAmountPerTick * 2);
            }
            else
            {
                mob.LoseHealth(ConversionDamage * 2);
            }
        }
    }
}
