using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Enemies;
using TheCure.Managers;
using TheCure.Weapons;

namespace TheCure
{
    public class Zombie : Enemy
    {
        private ZombieAnimationState _currentState;

        public Zombie() : base(
            textureName: "Zombie-Walk",
            speed: Settings.GetValue(SettingsConst.ZOMBIE.SPEED),
            startHealth: Settings.GetValue(SettingsConst.ZOMBIE.START_HEALTH),
            maxHealth: Settings.GetValue(SettingsConst.ZOMBIE.MAX_HEALTH),
            frameCount: 7,
            frameRate: 5f,
            scale: 2f
        )
        {
            _stagger = Settings.GetValue(SettingsConst.ZOMBIE.STAGGER);
            _attackDamage = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_DAMAGE);
            _attackCooldown = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_COOL_DOWN);
        }
        public override void Load()
        {
            base.Load();

            ((CircleCollider)collider).Center = _spawnPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, BecomeFriendly);
            SyncHealthBarPosition();

            SwitchAnimation("Zombie-Dead", 11, 5f, false, true);
            _currentState = ZombieAnimationState.Spawn;
            _isSpawning = true;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            LastHealed += deltaTime;
            base.Update(gameTime);
        }



        private void BecomeFriendly()
        {
            if (_isDying)
                return;

            _isDying = true;
            Vector2 spawnPosition = ((CircleCollider)collider).Center;

            SwitchAnimation("Zombie-Dead", 11, 5f, false);
            SoundManager.Get().PlayZombieDeath();
            _currentState = ZombieAnimationState.Dead;

            _onDeathComplete = () =>
            {
                var gm = GameManager.Get();
                gm.AddGameObject(new Friendly(FriendlyWeapons.HandGun, spawnPosition));
                gm.RemoveGameObject(this);
                ScoreManager.Get().AddScore(100, "Zombie Healed");
            };
        }

        public override void Destroy()
        {
            if (_isDying)
                return;

            _isDying = true;

            SwitchAnimation("Zombie-Dead", 11, 5f, false);
            SoundManager.Get().PlayZombieDeath();
            _currentState = ZombieAnimationState.Dead;

            _onDeathComplete = null;
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet bullet)
            {
                if (bullet.IsHealing)
                {
                    GainHealth(bullet.Damage);
                    SoundManager.Get().PlayHeal();
                    LastHealed = 0f;
                }
                else
                {
                    SoundManager.Get().PlayZombieHit();
                    LoseHealth(bullet.Damage);
                }

                bullet.Destroy();
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

        protected override void UpdateAnimation()
        {
            if (_isDying || _isSpawning)
                return;

            if (_attackNextCombat)
            {
                if (_currentState != ZombieAnimationState.Attack)
                {
                    SwitchAnimation("Zombie-Atk", 7, 8f, true);
                    _currentState = ZombieAnimationState.Attack;
                }
            }
            else
            {
                if (_currentState != ZombieAnimationState.Walk)
                {
                    SwitchAnimation("Zombie-Walk", 7, 5f, true);
                    _currentState = ZombieAnimationState.Walk;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = _isFlashing ? _flashColor : Color.White;
            Rectangle destRect = GetAnimatedSpriteDestinationRectangle();

            DrawShadow(spriteBatch, destRect);

            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }

        protected override void OnSpawnFinish()
        {
            SwitchAnimation("Zombie-Walk", 7, 5f, true);
            _currentState = ZombieAnimationState.Walk;
        }

        protected override void OnDeathFinish()
        {
            _onDeathComplete?.Invoke();
            if (_onDeathComplete == null)
            {
                ScoreManager.Get().AddScore(50, "Zombie Killed");
                base.Destroy();
            }
        }

        public void Spawn(Vector2 position)
        {
            _spawnPosition = position;
        }
    }

    enum ZombieAnimationState
    {
        Spawn,
        Walk,
        Attack,
        Dead
    }
}
