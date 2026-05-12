using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Enemies;
using TheCure.Engine.Managers;
using TheCure.Entities;
using TheCure.Managers;
using TheCure.Weapons;

namespace TheCure
{
    public class Friendly : LivingEntity
    {
        private Vector2 _spawnPosition;
        private Vector2 _previousCenter;

        private BaseWeapon _weapon;
        private float _sizeMultiplier;

        private FriendlyState _currentState;

        private Texture2D _idleTexture;
        private Texture2D _runTexture;
        private Texture2D _hitTexture;

        private const float BaseRadius = 120f;
        private const float RingSpacing = 75f;

        private const float MaxMoveSpeed = 120f;
        private const float Steering = 5f;
        private const float StopDistance = 4f;

        private const float SeparationDistance = 42f;
        private const float SeparationStrength = 22f;

        private const float IdleThreshold = 0.15f;
        private const float AttackRange = 300f;
        private const float CommandAttackRange = 460f;
        private const float CommandEnemyPriorityRadius = 420f;

        private enum FriendlyState
        {
            Idle,
            Run,
            Hit
        }

        public Friendly(FriendlyWeapons weaponType, Vector2 position)
            : base(
                textureName: "Character-Unknown-Idle",
                speed: Settings.GetValue(SettingsConst.FRIENDLY.MOVE_SPEED),
                startHealth: Settings.GetValue(SettingsConst.FRIENDLY.START_HEALTH),
                maxHealth: Settings.GetValue(SettingsConst.FRIENDLY.MAX_HEALTH),
                frameCount: 6,
                frameRate: 6f,
                scale: 1.7f
            )
        {
            collider = new CircleCollider(position, 16f);
            SetCollider(collider);

            _spawnPosition = position;
            _velocity = Vector2.Zero;
            _sizeMultiplier = Settings.GetValue(SettingsConst.FRIENDLY.SIZE);
            _velocity = Vector2.Zero;
            _sizeMultiplier = Settings.GetValue(SettingsConst.FRIENDLY.SIZE);

            switch (weaponType)
            {
                case FriendlyWeapons.HandGun:
                    _weapon = new Handgun();
                    break;

                default:
                    _weapon = new Handgun();
                    break;
            }
        }

        public override void Load()
        {
            base.Load();

            var content = ContentsManager.Get().GetContent();

            _idleTexture = content.Load<Texture2D>("Character-Unknown-Idle");
            _runTexture = content.Load<Texture2D>("Character-Unknown-Run");
            _hitTexture = content.Load<Texture2D>("Character-Unknown-Idle-Shot");

            ((CircleCollider)collider).Center = _spawnPosition;

            SetAnimation(_idleTexture, 5, 1f, true);

            SetHealthBar(_idleTexture, _maxHealth, _startHealth, Destroy, null);
            SyncHealthBarPosition();

            StatManager.Get().UpdateFriendlyStats(this);
            StatManager.Get().UpdateFriendlyStats(this);
        }

        private void SetAnimation(Texture2D texture, int frames, float fps, bool loop)
        {
            int frameWidth = texture.Width / frames;
            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop);
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var gm = GameManager.Get();
            var commandManager = CommandManager.Get();

            _previousCenter = ((CircleCollider)collider).Center;

            bool hasCommandPosition = commandManager.TryGetFriendlyCommandPosition(this, out Vector2 commandTarget);
            Vector2 target = hasCommandPosition
                ? commandTarget
                : GetRingTarget(gm);

            if (!hasCommandPosition || !commandManager.IsFriendlyCommandHolding())
            {
                target += GetSeparation(gm);
            }

            MoveTo(target, dt);
            Attack(gameTime);

            Vector2 movement = ((CircleCollider)collider).Center - _previousCenter;

            UpdateState(movement);

            _animatedSprite?.Update(gameTime);

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet bullet && bullet.IsHealing)
            {
                if (!_healthBar.IsMaxHealth)
                {
                    GainHealth(1);
                    tmp.Destroy();
                }
            }

            if (tmp is Wall wall)
            {
                Vector2 collisionNormal = wall.ResolveCircleCollision((CircleCollider)collider, _previousCenter);
                if (collisionNormal != Vector2.Zero)
                {
                    float velocityIntoWall = Vector2.Dot(_velocity, collisionNormal);
                    if (velocityIntoWall < 0f)
                    {
                        _velocity -= collisionNormal * velocityIntoWall;
                    }
                }
            }

            base.OnCollision(tmp);
        }

        private Vector2 GetRingTarget(GameManager gm)
        {
            var list = gm.Friendlies;

            int index = list.IndexOf(this);
            if (index < 0)
                return _spawnPosition;

            Vector2 player = PlayerManager.Get().Player.GetPosition();

            int ring = 0;
            int spots = 6;
            int start = 0;

            while (index >= start + spots)
            {
                start += spots;
                ring++;
                spots += 6;
            }

            int slot = index - start;

            float angleStep = MathHelper.TwoPi / spots;
            float angle = slot * angleStep - MathHelper.PiOver2;

            float radius = BaseRadius + ring * RingSpacing;

            return player + new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * radius;
        }

        private Vector2 GetSeparation(GameManager gm)
        {
            Vector2 force = Vector2.Zero;

            foreach (var other in gm.Friendlies)
            {
                if (other == this)
                    continue;

                if (other == null || other.collider == null)
                    continue;

                Vector2 diff = ((CircleCollider)collider).Center - ((CircleCollider)other.collider).Center;
                float dist = diff.Length();

                if (dist <= 0.01f || dist > SeparationDistance)
                    continue;

                diff /= dist;

                float strength = 1f - dist / SeparationDistance;
                force += diff * strength * SeparationStrength;
            }

            return force;
        }

        private void MoveTo(Vector2 target, float dt)
        {
            Vector2 toTarget = target - ((CircleCollider)collider).Center;
            float dist = toTarget.Length();

            if (dist < StopDistance)
            {
                _velocity = Vector2.Zero;
                return;
            }

            toTarget /= dist;

            float speed = Math.Min(dist * 2f, MaxMoveSpeed);
            Vector2 desired = toTarget * speed;

            float blend = MathHelper.Clamp(Steering * dt, 0f, 1f);
            _velocity = Vector2.Lerp(_velocity, desired, blend);

            if (_velocity.LengthSquared() < 0.01f)
                _velocity = Vector2.Zero;

            ((CircleCollider)collider).Center += _velocity * dt;
        }

        private void UpdateState(Vector2 movement)
        {
            if (_currentState == FriendlyState.Hit)
                return;

            if (movement.Length() > IdleThreshold)
                SetState(FriendlyState.Run);
            else
                SetState(FriendlyState.Idle);
        }

        private void SetState(FriendlyState state)
        {
            if (_currentState == state)
                return;
            if (_currentState == state)
                return;

            _currentState = state;

            switch (state)
            {
                case FriendlyState.Run:
                    SetAnimation(_runTexture, 8, 3f, true);
                    break;

                case FriendlyState.Hit:
                    SetAnimation(_hitTexture, 6, 6f, false);
                    break;

                default:
                    SetAnimation(_idleTexture, 5, 1f, false);
                    break;
            }
        }

        private void Attack(GameTime gameTime)
        {
            if (!_weapon.CanFire)
            {
                _weapon.UpdateCoolDown(gameTime);
                return;
            }

            var commandManager = CommandManager.Get();
            Vector2? commandTarget = commandManager.IsFriendlyCommandActive()
                ? commandManager.GetFriendlyCommandTarget()
                : null;

            Enemy enemy = GetNearestEnemy(commandTarget);

            if (enemy != null)
            {
                float dist = Vector2.Distance(((CircleCollider)enemy.collider).Center, ((CircleCollider)collider).Center);
                float range = commandTarget.HasValue ? CommandAttackRange : AttackRange;

                if (dist < range)
                {
                    Vector2 dir = ((CircleCollider)enemy.collider).Center - ((CircleCollider)collider).Center;

                    if (dir.LengthSquared() > 0.0001f)
                    {
                        dir.Normalize();
                        _weapon.Fire(((CircleCollider)collider).Center, dir);
                    }
                }
                else if (commandTarget.HasValue)
                {
                    Vector2 toCommandTarget = commandTarget.Value - ((CircleCollider)collider).Center;
                    if (toCommandTarget.LengthSquared() > 24f * 24f && toCommandTarget.Length() < CommandAttackRange)
                    {
                        toCommandTarget.Normalize();
                        _weapon.Fire(((CircleCollider)collider).Center, toCommandTarget);
                    }
                }

                _weapon.UpdateCoolDown(gameTime);
            }
        }

        private Enemy GetNearestEnemy(Vector2? commandTarget)
        {
            Enemy best = null;
            float bestDist = float.MaxValue;

            foreach (var enemy in GameManager.Get().Enemies)
            {
                if (enemy == null) continue;

                float dist = Vector2.Distance(((CircleCollider)enemy.collider).Center, ((CircleCollider)collider).Center);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = enemy;
                }
            }

            return best;
        }

        public void SetSizeMultiplier(float sizeMultiplier)
        {
            _sizeMultiplier = sizeMultiplier;
        }

        public void SetWeaponDamage(float damage)
        {
            _weapon.SetDamageMultiplier(damage);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = _isFlashing ? _flashColor : Color.White;
            Rectangle destinationRectangle = GetAnimatedSpriteDestinationRectangle(_sizeMultiplier);

            DrawShadow(spriteBatch, destinationRectangle);
            DrawAnimatedSprite(spriteBatch, tint, _sizeMultiplier);

            base.Draw(gameTime, spriteBatch);
        }
    }
}