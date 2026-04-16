using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    public class Friendly : Mob
    {
        private Vector2 _startPosition;
        private Vector2 _previousCenter;
        private Vector2 _velocity;
        private Vector2 _formationAnchor;
        private bool _hasFormationAnchor;
        private Vector2 _facingDirection = Vector2.UnitX;

        private BaseWeapon _weapon;

        // ===== FORMATION SETTINGS (VERSION 1) =====
        private const int RingSize = 6;
        private const float BaseRadius = 130f;
        private const float RingSpacing = 80f;
        private const float AnchorCatchupSpeed = 3.5f;
        private const float SteeringResponsiveness = 6f;
        private const float SlowRadius = 90f;
        private const float FriendlySeparationStrength = 36f;

        // ===== ANIMATION (VERSION 2) =====
        private AnimatedSprite _animatedSprite;
        private FriendlyState _currentState;

        private enum FriendlyState
        {
            Idle,
            Run,
            Hit
        }

        public Friendly(FriendlyWeapons friendlyWeapon) : base(
            textureName: "Character-Unknown-Idle",
            speed: Settings.GetValue(SettingsConst.FRIENDLY.MOVE_SPEED),
            startHealth: Settings.GetValue(SettingsConst.FRIENDLY.START_HEALTH),
            maxHealth: Settings.GetValue(SettingsConst.FRIENDLY.MAX_HEALTH),
            frameCount: 6,
            frameRate: 6f,
            scale: 1.7f
        )
        {
            _collider = new CircleCollider(Vector2.Zero, BaseRadius);
            switch (friendlyWeapon)
            {
                case FriendlyWeapons.HandGun:
                    _weapon = new Handgun();
                    break;
            }

            _velocity = Vector2.Zero;
        }

        public Friendly(FriendlyWeapons friendlyWeapons, Vector2 position)
            : this(friendlyWeapons)
        {
            _startPosition = position;
            _formationAnchor = position;
            _hasFormationAnchor = true;

            GameManager.GetGameManager().Friendlies.Add(this);
        }

        public override void Load(ContentManager content)
        {
            SwitchAnimation("Character-Unknown-Idle", 6, 6f, true);

            SetHealthBar(
                content.Load<Texture2D>("Character-Unknown-Idle"),
                _maxHealth,
                _startHealth,
                Destroy,
                null
            );

            _collider.Center = _startPosition;

            base.Load(content);
        }

        // ===== ANIMATION SWITCH =====
        private void SwitchAnimation(string name, int frames, float fps, bool loop)
        {
            var texture = GameManager.GetGameManager()._content.Load<Texture2D>(name);
            int frameWidth = texture.Width / frames;

            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var gm = GameManager.GetGameManager();

            Vector2 playerPos = gm.Player.GetPosition().Center.ToVector2();

            _previousCenter = _collider.Center;

            if (gm.Friendlies.Count == 0)
                return;

            // ===== FORMATION ANCHOR (VERSION 1) =====
            if (!_hasFormationAnchor)
            {
                _formationAnchor = playerPos;
                _hasFormationAnchor = true;
            }

            float anchorBlend = MathHelper.Clamp(AnchorCatchupSpeed * deltaTime, 0f, 1f);
            _formationAnchor = Vector2.Lerp(_formationAnchor, playerPos, anchorBlend);

            Vector2 formationTarget = GetFormationTarget(gm, _formationAnchor);

            formationTarget = KeepOutsidePlayer(gm.Player, formationTarget, 18f);
            formationTarget += GetSeparationOffset(gm);

            MoveTowards(formationTarget, deltaTime);

            Attack(gameTime);

            // ===== FACING =====
            Vector2 movement = _collider.Center - _previousCenter;
            if (movement.LengthSquared() > 0.0001f)
                _facingDirection = Vector2.Normalize(movement);

            UpdateState();

            _animatedSprite?.Update(gameTime);

            base.Update(gameTime);
        }

        // ===== FORMATION (VERSION 1) =====
        private Vector2 GetFormationTarget(GameManager gm, Vector2 anchor)
        {
            int index = gm.Friendlies.IndexOf(this);
            if (index < 0) return anchor;

            int ringNumber = index / RingSize;
            int indexInRing = index % RingSize;

            float angleStep = MathHelper.TwoPi / RingSize;
            float ringOffset = ringNumber % 2 == 0 ? 0f : angleStep * 0.5f;

            float angle = indexInRing * angleStep + ringOffset - MathHelper.PiOver2;
            float radius = BaseRadius + ringNumber * RingSpacing;

            Vector2 offset = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * radius;

            return anchor + offset;
        }

        private Vector2 KeepOutsidePlayer(Player player, Vector2 pos, float padding)
        {
            Rectangle b = player.GetPosition();

            float left = b.Left - padding;
            float right = b.Right + padding;
            float top = b.Top - padding;
            float bottom = b.Bottom + padding;

            if (pos.X < left) pos.X = left;
            if (pos.X > right) pos.X = right;
            if (pos.Y < top) pos.Y = top;
            if (pos.Y > bottom) pos.Y = bottom;

            return pos;
        }

        private Vector2 GetSeparationOffset(GameManager gm)
        {
            Vector2 offset = Vector2.Zero;
            float desired = 40f;

            foreach (var other in gm.Friendlies)
            {
                if (other == this) continue;

                Vector2 away = _collider.Center - other._collider.Center;
                if (away.LengthSquared() < 0.0001f) continue;

                float dist = away.Length();
                if (dist >= desired) continue;

                away /= dist;
                float strength = 1f - (dist / desired);

                offset += away * (strength * FriendlySeparationStrength);
            }

            return offset;
        }

        private void MoveTowards(Vector2 target, float dt)
        {
            Vector2 dir = target - _collider.Center;
            float dist = dir.Length();

            Vector2 desired = Vector2.Zero;

            if (dist > 0.5f)
            {
                dir /= dist;
                float speed = MathHelper.Clamp(dist / SlowRadius, 0f, 1f);
                desired = dir * (_speed * speed);
            }

            float blend = MathHelper.Clamp(SteeringResponsiveness * dt, 0f, 1f);
            _velocity = Vector2.Lerp(_velocity, desired, blend);

            _collider.Center += _velocity * dt;
        }

        // ===== ATTACK (VERSION 1) =====
        private void Attack(GameTime gameTime)
        {
            if (!_weapon.CanFire)
                return;

            Mob enemy = GetNearestEnemy();

            if (enemy == null) return;

            Vector2 target = enemy._collider.Center;
            float dist = Vector2.Distance(target, _collider.Center);

            if (dist < 300f)
            {
                Vector2 dir = LinePieceCollider.GetDirection(_collider.Center, target);
                _weapon.Fire(_collider.Center, dir);
            }

            _weapon.UpdateCoolDown(gameTime);
        }

        private Mob GetNearestEnemy()
        {
            var enemies = GameManager.GetGameManager().Enemies;

            Mob closest = null;
            float best = float.MaxValue;

            foreach (var e in enemies)
            {
                if (e == null) continue;

                if (e is Zombie z && z.LastHealed < 3f) continue;

                float d = Vector2.Distance(e._collider.Center, _collider.Center);

                if (d < best)
                {
                    best = d;
                    closest = e;
                }
            }

            return closest;
        }

        // ===== ANIMATION STATE =====
        private void UpdateState()
        {
            if (_currentState == FriendlyState.Hit)
                return;

            if ((_collider.Center - _previousCenter).LengthSquared() > 0.01f)
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
                    SwitchAnimation("Character-Unknown-Run", 6, 8f, true);
                    break;

                case FriendlyState.Hit:
                    SwitchAnimation("Character-Unknown-Idle-Shot", 4, 10f, false);
                    break;

                default:
                    SwitchAnimation("Character-Unknown-Idle", 6, 6f, true);
                    break;
            }
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
                wall.ResolveCircleCollision(_collider, _previousCenter);
            }

            base.OnCollision(tmp);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animatedSprite?.Draw(
                spriteBatch,
                _collider.Center,
                Color.LightBlue,
                0,
                2f
            );

            base.Draw(gameTime, spriteBatch);
        }
    }
}