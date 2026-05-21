using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;
using TheCure.Enemies;
using TheCure.Entities;

namespace TheCure.BaseObjects.Traps
{
    public class HealBombTrap : Trap
    {
        private const int HealAmountPerTick = 5;
        private const float HealTickInterval = 0.4f;
        private const float HealRadius = 120f;
        private const int ConversionDamage = 10;

        private float _healTickTimer = HealTickInterval;

        private bool _isExploding = false;
        private float _throwAnimTime = 0f;
        private const float ThrowAnimDuration = 6 * 0.08f; // frames * fps

        public HealBombTrap(Vector2 position, float duration = 15f)
            : base(position, duration)
        {
            _baseColor = Color.LimeGreen;
            _currentColor = Color.LimeGreen;
        }

        public override void Load()
        {
            SwitchAnimation("Bomb-throw", 6, 12f, false);

            base.Load();
        }

        protected override void UpdateTrap(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!_isExploding)
            {
                if (_animatedSprite != null && _animatedSprite.IsFinished)
                {
                    _isExploding = true;

                    SwitchAnimation("Bomb-explosion", 6, 12f, false);
                }
            }
            else
            {
                if (_animatedSprite != null && _animatedSprite.IsFinished)
                {
                    Destroy();
                }
            }


            _healTickTimer -= deltaTime;

            if (_healTickTimer <= 0f)
            {
                _healTickTimer = HealTickInterval;

                var gm = GameManager.Get();

                if (gm.Friendlies != null)
                {
                    foreach (var f in gm.Friendlies)
                    {
                        if (f?.collider is CircleCollider fc &&
                            Vector2.Distance(fc.Center, _collider.Center) < HealRadius)
                        {
                            f.GainHealth(HealAmountPerTick);
                        }
                    }
                }

                if (gm.Enemies != null)
                {
                    foreach (var e in gm.Enemies)
                    {
                        if (e?.collider is CircleCollider ec &&
                            Vector2.Distance(ec.Center, _collider.Center) < HealRadius)
                        {
                            e.LoseHealth(ConversionDamage);
                        }
                    }
                }
            }

            float pulse = 0.6f + (0.4f * (float)Math.Sin(_elapsedTime * 3));
            _currentColor = _baseColor * pulse;
        }

        protected override void OnTrapHit(LivingEntity target)
        {
            if (target is Friendly friendly)
                friendly.GainHealth(HealAmountPerTick * 2);
            else if (target is Enemy enemy)
                enemy.LoseHealth(ConversionDamage * 2);
        }
    }
}