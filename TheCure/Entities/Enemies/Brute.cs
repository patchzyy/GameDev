using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Enemies;
using TheCure.Managers;

namespace TheCure
{
    public class Brute : Enemy
    {
        private BruteAnimationState _currentState;

        public Brute() : base(
            textureName: "Zombie-Walk",
            speed: 30f,
            startHealth: 15f,
            maxHealth: 15f,
            frameCount: 7,
            frameRate: 3.5f,
            scale: 3.1f
        )
        {
            _stagger = 1.2f;
            _attackDamage = 3;
            _attackCooldown = 2f;
        }

        public override void Load()
        {
            base.Load();

            ((CircleCollider)collider).Center = _spawnPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);
            SyncHealthBarPosition();

            SwitchAnimation("Zombie-Dead", 11, 3.5f, false, true);
            _currentState = BruteAnimationState.Spawn;
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

        public override void Destroy()
        {
            if (_isDying)
                return;

            _isDying = true;

            SwitchAnimation("Zombie-Dead", 11, 3.5f, false);
            _currentState = BruteAnimationState.Dead;
        }

        protected override void UpdateAnimation()
        {
            if (_isDying || _isSpawning)
                return;

            if (_attackNextCombat)
            {
                if (_currentState != BruteAnimationState.Attack)
                {
                    SwitchAnimation("Zombie-Atk", 7, 5f, true);
                    _currentState = BruteAnimationState.Attack;
                }
            }
            else
            {
                if (_currentState != BruteAnimationState.Walk)
                {
                    SwitchAnimation("Zombie-Walk", 7, 3.5f, true);
                    _currentState = BruteAnimationState.Walk;
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
            DrawShadow(spriteBatch, destRect, 0.18f, 0.10f);

            Color tint = _isFlashing ? _flashColor : Color.White;
            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }

        protected override void OnSpawnFinish()
        {
            SwitchAnimation("Zombie-Walk", 7, 3.5f, true);
            _currentState = BruteAnimationState.Walk;
        }

        protected override void OnDeathFinish()
        {
            _onDeathComplete?.Invoke();
            if (_onDeathComplete == null)
            {
                ScoreManager.Get().AddScore(100, "Brute Killed");
                base.Destroy();
            }
        }
    }

    enum BruteAnimationState
    {
        Spawn,
        Walk,
        Attack,
        Dead
    }
}