using System;
using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.Entities;

namespace TheCure.Enemies
{
    public abstract class Enemy : LivingEntity
    {
        protected float _attackCoolDown;
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

        protected System.Collections.Generic.List<Vector2> _currentPath;
        protected float _pathUpdateTimer;

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

            //Niet elke frame het pad berekenen, dat scheelt rekenkracht!
            _pathUpdateTimer -= deltaTime;
            if (_pathUpdateTimer <= 0f)
            {
                _currentPath = Pathfinder.FindPath(((CircleCollider)collider).Center, targetPosition);
                // Willekeurige timer zodat niet alle zombies alles aanroepen,
                // anders krijg je namelijk lagspikes
                _pathUpdateTimer = 0.5f + (float)GameManager.Get().RNG.NextDouble() * 0.5f;
            }

            Vector2 direction = Vector2.Zero;

            if (_currentPath != null && _currentPath.Count > 0)
            {
                // Navigeer langs t pa
                Vector2 currentWaypoint = _currentPath[0];
                direction = currentWaypoint - ((CircleCollider)collider).Center;

                // Zijn we dichtbij genoeg? Pak de volgende waypoint
                if (direction.LengthSquared() < 64f * 0.5f * 64f * 0.5f) // halverwege
                {
                    _currentPath.RemoveAt(0);
                    if (_currentPath.Count > 0)
                    {
                        currentWaypoint = _currentPath[0];
                        direction = currentWaypoint - ((CircleCollider)collider).Center;
                    }
                }
            }
            else
                direction = targetPosition - ((CircleCollider)collider).Center;

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
            _attackTimer = _attackCoolDown;
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
                OnDeathFinish();

            return true;
        }

        protected abstract void UpdateAnimation();
        protected abstract void OnSpawnFinish();
        protected abstract void OnDeathFinish();
    }
}