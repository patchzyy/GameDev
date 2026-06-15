using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Enemies;
using TheCure.Managers;

namespace TheCure
{
    public class BabyZombie : Enemy
    {
        private BabyZombieAnimationState _currentState;

        public BabyZombie() : base(
            textureName: "Zombie-Walk",
            speed: Settings.GetValue(SettingsConst.ZOMBIE.SPEED) *
                   Settings.GetValue(SettingsConst.BABY_ZOMBIE.MOVE_SPEED_MULTIPLIER),
            startHealth: Settings.GetValue(SettingsConst.ZOMBIE.START_HEALTH) *
                         Settings.GetValue(SettingsConst.BABY_ZOMBIE.HEALTH_MULTIPLIER),
            maxHealth: Settings.GetValue(SettingsConst.ZOMBIE.START_HEALTH) *
                       Settings.GetValue(SettingsConst.BABY_ZOMBIE.HEALTH_MULTIPLIER),
            frameCount: 7,
            frameRate: 8f,
            scale: Settings.GetValue(SettingsConst.BABY_ZOMBIE.SCALE)
        )
        {
            _stagger = 0.35f;
            _attackDamage = 1f;
            _attackCoolDown = 1.1f;
        }

        public override void Load()
        {
            base.Load();

            ((CircleCollider)collider).Center = _spawnPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);
            SyncHealthBarPosition();

            SwitchAnimation("Zombie-Dead", 11, 8f, false, true);
            _currentState = BabyZombieAnimationState.Spawn;
            _isSpawning = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet bullet)
            {
                if (!bullet.IsHealing)
                {
                    SoundManager.Get().PlayZombieHit();
                    LoseHealth(bullet.Damage);
                    bullet.Destroy();
                }
            }

            if ((tmp is Friendly || tmp is Player) && _currentTarget == null)
            {
                _currentTarget = tmp;
                _attackNextCombat = true;
            }

            if (tmp is Wall wall)
            {
                wall.ResolveCircleCollision((CircleCollider)collider, _previousCenter);
            }

            base.OnCollision(tmp);
        }

        public override void Destroy()
        {
            if (_isDying)
                return;

            _isDying = true;

            SwitchAnimation("Zombie-Dead", 11, 8f, false);
            SoundManager.Get().PlayZombieDeath();
            _currentState = BabyZombieAnimationState.Dead;
        }

        protected override void UpdateAnimation()
        {
            if (_isDying || _isSpawning)
                return;

            if (_attackNextCombat)
            {
                if (_currentState != BabyZombieAnimationState.Attack)
                {
                    SwitchAnimation("Zombie-Atk", 7, 10f, true);
                    _currentState = BabyZombieAnimationState.Attack;
                }
            }
            else
            {
                if (_currentState != BabyZombieAnimationState.Walk)
                {
                    SwitchAnimation("Zombie-Walk", 7, 8f, true);
                    _currentState = BabyZombieAnimationState.Walk;
                }
            }
        }

        public void Spawn(Vector2 position)
        {
            _spawnPosition = position;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle destRect = GetAnimatedSpriteDestinationRectangle();
            DrawShadow(spriteBatch, destRect, 0.10f, 0.06f);

            Color tint = _isFlashing ? _flashColor : Color.White;
            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }

        protected override void OnSpawnFinish()
        {
            SwitchAnimation("Zombie-Walk", 7, 8f, true);
            _currentState = BabyZombieAnimationState.Walk;
        }

        protected override void OnDeathFinish()
        {
            _onDeathComplete?.Invoke();

            if (_onDeathComplete == null)
            {
                ScoreManager.Get().AddScore(25, "Baby Zombie Killed");
                base.Destroy();
            }
        }
    }

    enum BabyZombieAnimationState
    {
        Spawn,
        Walk,
        Attack,
        Dead
    }
}