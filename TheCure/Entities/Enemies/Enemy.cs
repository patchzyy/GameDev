using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Collision;

namespace TheCure.Mobs
{
    public abstract class Enemy : LivingEntity
    {
        protected GameObject _target;
        protected EnemyState _state;

        protected Action _onDeathComplete;

        protected float _attackCooldown;
        protected float _attackTimer;
        protected int _attackDamage;

        protected float _stagger;
        protected float _staggerTimer;

        protected float _attackRange = 40f;

        protected Vector2 _previousCenter;

        protected Enemy(
            string textureName,
            float speed,
            float startHealth,
            float maxHealth,
            int frameCount = 1,
            float frameRate = 1f,
            bool isLooping = true,
            float scale = 1f)
            : base(textureName, speed, startHealth, maxHealth, frameCount, frameRate, isLooping, scale)
        {
        }


        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _previousCenter = _collider.Center;

            UpdateTimers(dt);

            if (_target == null)
                _target = GameManager.GetGameManager().Player;

            //_animatedSprite?.Update(gameTime);

            switch (_state)
            {
                case EnemyState.Spawning:
                    UpdateSpawn(gameTime);
                    return;

                case EnemyState.Dying:
                    UpdateDeath(gameTime);
                    return;

                case EnemyState.Walk:
                    HandleCombatAndMovement(dt);
                    UpdateFacingDirection(_collider.Center - _previousCenter);
                    break;
            }

            base.Update(gameTime);
        }

        protected virtual void UpdateSpawn(GameTime gameTime)
        {
            _animatedSprite.Update(gameTime);

            if (_animatedSprite.IsFinished)
            {
                _state = EnemyState.Walk;
            }
        }

        protected virtual void UpdateDeath(GameTime gameTime)
        {
            _animatedSprite.Update(gameTime);

            if (!_animatedSprite.IsFinished)
                return;

            // only run once
            if (_onDeathComplete != null)
            {
                _onDeathComplete.Invoke();
                _onDeathComplete = null;
                return;
            }

            GameManager.GetGameManager().RemoveGameObject(this);
        }

        protected virtual void HandleCombatAndMovement(float dt)
        {
            if (_target == null)
            {
                Move(dt);
                return;
            }

            float dist = Vector2.Distance(
                _collider.Center,
                _target.GetCollider().GetBoundingBox().Center.ToVector2()
            );

            if (dist <= _attackRange)
            {                
                if (_attackTimer <= 0f)
                    Attack(dt);
                _state = EnemyState.Attack;
            }
            else
            {
                _state = EnemyState.Walk;
                Move(dt);
            }
        }

        private void UpdateTimers(float dt)
        {
            if (_attackTimer > 0f)
                _attackTimer -= dt;

            if (_staggerTimer > 0f)
                _staggerTimer -= dt;
        }

        protected virtual void Attack(float dt)
        {
            if (_attackTimer > 0f || _target == null)
                return;

            _target.LoseHealth(_attackDamage);

            _attackTimer = _attackCooldown;
        }

        protected abstract void Move(float dt);

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Player || tmp is Friendly)
            {
                if (_target == null)
                    _target = tmp;
            }

            if (tmp is Bullet bullet)
            {
                if (bullet.IsHealing)
                {
                    GainHealth(1);
                    LastHealed = 0f;
                }
                else
                    LoseHealth(bullet.Damage);
                bullet.Destroy();
            }

            if (tmp is Wall wall)
            {
                wall.ResolveCircleCollision(_collider, _previousCenter);
            }

            base.OnCollision(tmp);
        }
    }
    public enum EnemyState
    {
        Spawning,
        Walk,
        Attack,
        Dying
    }
}