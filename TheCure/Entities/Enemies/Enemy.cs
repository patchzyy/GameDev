using System;
using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.Entities;

namespace TheCure.Enemies
{
    public abstract class Enemy : LivingEntity
    {
        protected float _attackCooldown;
        protected float _attackTimer;
        protected float _attackDamage;
        protected bool _attackNextCombat;

        protected float _stagger;
        protected float _staggerTimer;

        protected bool _isSpawning;
        protected bool _isDying;
        protected Action _onDeathComplete;

        protected GameObject _currentTarget;

        protected Vector2 _previousCenter;
        protected Vector2 _spawnPosition;

        public Enemy(string textureName, float speed, float startHealth, float maxHealth, int frameCount = 1,
            float frameRate = 1f, bool isLooping = true, float scale = 1f) : base(textureName, speed, startHealth,
            maxHealth, frameCount, frameRate, isLooping, scale)
        {
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _previousCenter = ((CircleCollider)collider).Center;
            UpdateKnockBack(deltaTime);

            if (HandleSpawning(gameTime))
                return;
            if (HandleDying(gameTime))
                return;

            if (_attackNextCombat)
                Attack(deltaTime);
            else
                Move(deltaTime);

            Vector2 movement = ((CircleCollider)collider).Center - _previousCenter;

            if (movement.LengthSquared() > 0.0001f)
                _facingDirection = Vector2.Normalize(movement);

            LastHealed += deltaTime;

            UpdateAnimation();
            base.Update(gameTime);
        }

        protected virtual void Move(float deltaTime)
        {
            if (_stagger > 0f && _attackNextCombat)
            {
                _stagger -= deltaTime;
                return;
            }

            Vector2 targetPosition = _currentTarget == null
                ? PlayerManager.Get().Player.GetPosition()
                : _currentTarget.GetCollider().GetBoundingBox().Center.ToVector2();

            Vector2 direction = targetPosition - ((CircleCollider)collider).Center;

            if (direction.LengthSquared() > 0.0001f)
            {
                direction.Normalize();
                ((CircleCollider)collider).Center += direction * (_speed / 2f) * deltaTime;
            }
        }

        protected virtual void Attack(float deltaTime)
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= deltaTime;
                return;
            }

            _currentTarget?.LoseHealth(_attackDamage);
            SoundManager.Get().PlayFriendlyHit();
            _attackNextCombat = false;
            _attackTimer = _attackCooldown;
            _currentTarget = null;
        }

        protected virtual bool HandleSpawning(GameTime gameTime)
        {
            if (!_isSpawning)
                return false;

            _animatedSprite?.Update(gameTime);
            base.Update(gameTime);

            if (_animatedSprite.IsFinished)
            {
                _isSpawning = false;
                OnSpawnFinish();
            }

            return true;
        }

        protected virtual bool HandleDying(GameTime gameTime)
        {
            if (!_isDying)
                return false;

            _animatedSprite?.Update(gameTime);
            base.Update(gameTime);

            if (_animatedSprite.IsFinished)
            {
                OnDeathFinish();
            }

            return true;
        }

        protected abstract void UpdateAnimation();
        protected abstract void OnSpawnFinish();
        protected abstract void OnDeathFinish();
    }
}