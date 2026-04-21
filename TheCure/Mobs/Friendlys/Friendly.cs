using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    public class Friendly : Mob
    {
        private Vector2 _spawnPosition;
        private Vector2 _previousCenter;
        private Vector2 _velocity;

        private BaseWeapon _weapon;

        private AnimatedSprite _animatedSprite;
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
            _collider = new CircleCollider(Vector2.Zero, 16f);

            _spawnPosition = position;
            _collider.Center = position;

            switch (weaponType)
            {
                case FriendlyWeapons.HandGun:
                    _weapon = new Handgun();
                    break;
            }

            GameManager.GetGameManager().Friendlies.Add(this);
        }

        public override void Load(ContentManager content)
        {
            _idleTexture = content.Load<Texture2D>("Character-Unknown-Idle");
            _runTexture = content.Load<Texture2D>("Character-Unknown-Run");
            _hitTexture = content.Load<Texture2D>("Character-Unknown-Idle-Shot");

            SetAnimation(_idleTexture, 5, 1f, true);

            SetHealthBar(
                _idleTexture,
                _maxHealth,
                _startHealth,
                Destroy,
                null
            );

            base.Load(content);
        }

        private void SetAnimation(Texture2D texture, int frames, float fps, bool loop)
        {
            int frameWidth = texture.Width / frames;
            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop);
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var gm = GameManager.GetGameManager();

            _previousCenter = _collider.Center;

            Vector2 target = GetRingTarget(gm);
            target += GetSeparation(gm);

            MoveTo(target, dt);

            Attack(gameTime);

            Vector2 movement = _collider.Center - _previousCenter;

            UpdateState(movement);

            _animatedSprite?.Update(gameTime);

            base.Update(gameTime);
        }

        private Vector2 GetRingTarget(GameManager gm)
        {
            var list = gm.Friendlies;

            int index = list.IndexOf(this);
            if (index < 0)
                return _spawnPosition;

            Vector2 player = gm.Player.GetPosition().Center.ToVector2();

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
                if (other == this) continue;

                Vector2 diff = _collider.Center - other._collider.Center;
                float dist = diff.Length();

                if (dist <= 0.01f || dist > SeparationDistance)
                    continue;

                diff /= dist;

                float strength = 1f - (dist / SeparationDistance);
                force += diff * strength * SeparationStrength;
            }

            return force;
        }

        private void MoveTo(Vector2 target, float dt)
        {
            Vector2 toTarget = target - _collider.Center;
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

            _collider.Center += _velocity * dt;
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
            if (_currentState == state) return;

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

            Mob enemy = GetNearestEnemy();
            if (enemy == null) return;

            float dist = Vector2.Distance(enemy._collider.Center, _collider.Center);

            if (dist < 300f)
            {
                Vector2 dir = Vector2.Normalize(enemy._collider.Center - _collider.Center);
                _weapon.Fire(_collider.Center, dir);
            }

            _weapon.UpdateCoolDown(gameTime);
        }

        private Mob GetNearestEnemy()
        {
            Mob best = null;
            float bestDist = float.MaxValue;

            foreach (var e in GameManager.GetGameManager().Enemies)
            {
                if (e == null) continue;

                float dist = Vector2.Distance(e._collider.Center, _collider.Center);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = e;
                }
            }

            return best;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = _isFlashing ? _flashColor : Color.White;
            _animatedSprite?.Draw(
                spriteBatch,
                _collider.Center,
                tint,
                0f,
                2f
            );

            base.Draw(gameTime, spriteBatch);
        }
    }
}