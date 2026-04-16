using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    public class Zombie : Mob
    {
        private float _attackCoolDown;
        private float _stagger;
        private int _attackDamage;

        private bool _attackNextCombat;
        private float _attackTimer;
        private GameObject _currentTarget;
        private Vector2 _previousCenter;
        private Vector2 _facingDirection = Vector2.UnitX;

        private ZombieAnimationState _currentState;
        private bool _isDying = false;
        private Action _onDeathComplete;

        public float LastHealed;

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
            _attackCoolDown = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_COOLDOWN);
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, BecomeFriendly);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _previousCenter = _collider.Center;
            
            if (_isDying)
            {
                _animatedSprite.Update(gameTime);

                if (_animatedSprite.IsFinished)
                {
                    _onDeathComplete?.Invoke();

                    if (_onDeathComplete == null)
                    {
                        GameManager.GetGameManager().AddScore(50, "Zombie Killed");
                        base.Destroy();
                    }
                }

                return;
            }

            // Movement / attack logic
            if (_attackNextCombat)
                Attack(deltaTime);
            else
                Move(deltaTime);

            Vector2 movement = _collider.Center - _previousCenter;

            if (movement.LengthSquared() > 0.0001f)
                _facingDirection = Vector2.Normalize(movement);

            LastHealed += deltaTime;

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

            if (direction != Vector2.Zero)
                direction.Normalize();

            _collider.Center += direction * (_speed / 2f) * deltaTime;
        }

        private void Attack(float deltaTime)
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= deltaTime;
                return;
            }

            _currentTarget.LoseHealth(_attackDamage);

            _attackNextCombat = false;
            _attackTimer = _attackCoolDown;
            _currentTarget = null;
        }

        private void BecomeFriendly()
        {
            if (_isDying)
                return;

            _isDying = true;

            SwitchAnimation("Zombie-Dead", 8, 3f, false);
            _currentState = ZombieAnimationState.Dead;

            _onDeathComplete = () =>
            {
                var gm = GameManager.GetGameManager();
                gm.AddGameObject(new Friendly(FriendlyWeapons.HandGun, _collider.Center));
                gm.RemoveGameObject(this);
                gm.AddScore(100, "Zombie Healed");
            };
        }

        public override void Destroy()
        {
            if (_isDying)
                return;

            _isDying = true;

            SwitchAnimation("Zombie-Dead", 8, 3f, false);
            _currentState = ZombieAnimationState.Dead;

            _onDeathComplete = null; // gewone death (geen friendly)
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet bullet)
            {
                if (bullet.IsHealing)
                {
                    GainHealth(1);
                    LastHealed = 0f;
                }
                else
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

        private void UpdateAnimation()
        {
            if (_isDying)
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

        private void SwitchAnimation(string name, int frames, float fps, bool loop)
        {
            var texture = GameManager.GetGameManager()._content.Load<Texture2D>(name);
            int frameWidth = texture.Width / frames;

            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop);
        }

        public void RandomMove()
        {
            var gameManager = GameManager.GetGameManager();
            var viewport = gameManager.Game.GraphicsDevice.Viewport;
            var rng = gameManager.RNG;

            Vector2 playerPos = gameManager.Player.GetPosition().Center.ToVector2();

            Vector2 spawn;
            float minDistance = 100f;

            do
            {
                spawn = new Vector2(
                    rng.Next(0, viewport.Width),
                    rng.Next(0, viewport.Height)
                );
            }
            while (Vector2.Distance(spawn, playerPos) < minDistance);

            _collider.Center = spawn;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.White;

            Rectangle destRect = GetAnimatedSpriteDestinationRectangle();

            DrawShadow(spriteBatch, destRect);

            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }
    }

    enum ZombieAnimationState
    {
        Walk,
        Attack,
        Dead
    }
}