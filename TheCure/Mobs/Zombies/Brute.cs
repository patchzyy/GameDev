using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;

namespace TheCure
{
    public class Brute : Mob
    {
        private float _attackCoolDown;
        private float _stagger;
        private int _attackDamage;

        private bool _attackNextCombat;
        private float _attackTimer;
        private GameObject _currentTarget;
        private Vector2 _previousCenter;
        private Vector2 _facingDirection = Vector2.UnitX;

        private BruteAnimationState _currentState;
        private bool _isSpawning = false;
        private bool _isDying = false;

        private Vector2 _spawnPosition;

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
            _attackCoolDown = 2f;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            _collider.Center = _spawnPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);

            SwitchAnimation("Zombie-Dead", 11, 6f, false, true);
            _currentState = BruteAnimationState.Spawn;
            _isSpawning = true;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousCenter = _collider.Center;

            if (_isSpawning)
            {
                _animatedSprite.Update(gameTime);

                if (_animatedSprite.IsFinished)
                {
                    _isSpawning = false;
                    SwitchAnimation("Zombie-Walk", 7, 3.5f, true);
                    _currentState = BruteAnimationState.Walk;
                }

                return;
            }

            if (_isDying)
            {
                _animatedSprite.Update(gameTime);

                if (_animatedSprite.IsFinished)
                {
                    GameManager.GetGameManager().AddScore(75, "Brute Killed");
                    base.Destroy();
                }

                return;
            }

            if (_attackNextCombat)
            {
                Attack(deltaTime);
            }
            else
            {
                Move(deltaTime);
            }

            Vector2 movement = _collider.Center - _previousCenter;
            if (movement.LengthSquared() > 0.0001f)
            {
                _facingDirection = Vector2.Normalize(movement);
            }

            UpdateAnimation();

            base.Update(gameTime);
        }

        private void Move(float deltaTime)
        {
            if (_stagger > 0f && _attackNextCombat)
            {
                _stagger -= deltaTime;
                return;
            }

            Vector2 targetPosition = _currentTarget == null
                ? GameManager.GetGameManager().Player.GetPosition().Center.ToVector2()
                : _currentTarget.GetCollider().GetBoundingBox().Center.ToVector2();

            Vector2 direction = targetPosition - _collider.Center;

            if (direction.LengthSquared() > 0.0001f)
            {
                direction.Normalize();
                _collider.Center += direction * (_speed / 2f) * deltaTime;
            }
        }

        private void Attack(float deltaTime)
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= deltaTime;
                return;
            }

            if (_currentTarget != null)
            {
                _currentTarget.LoseHealth(_attackDamage);
            }

            _attackNextCombat = false;
            _attackTimer = _attackCoolDown;
            _currentTarget = null;
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet bullet)
            {
                if (!bullet.IsHealing)
                {
                    LoseHealth(1);
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
                wall.ResolveCircleCollision(_collider, _previousCenter);
            }

            base.OnCollision(tmp);
        }

        public override void Destroy()
        {
            if (_isDying)
                return;

            _isDying = true;

            SwitchAnimation("Zombie-Dead", 11, 6f, false);
            _currentState = BruteAnimationState.Dead;
        }

        private void UpdateAnimation()
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

        private void SwitchAnimation(string name, int frames, float fps, bool loop, bool reverse = false)
        {
            var texture = GameManager.GetGameManager()._content.Load<Texture2D>(name);
            int frameWidth = texture.Width / frames;

            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop, reverse);
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
    }

    enum BruteAnimationState
    {
        Spawn,
        Walk,
        Attack,
        Dead
    }
}