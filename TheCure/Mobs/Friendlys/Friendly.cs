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
        private float _radius;
        private Vector2 _previousCenter;
        private Vector2 _velocity;
        private Vector2 _formationAnchor;
        private bool _hasFormationAnchor;
        private Vector2 _facingDirection = Vector2.UnitX;

        private BaseWeapon _weapon;
        private const int RingSize = 6;
        private const float BaseRadius = 130f;
        private const float RingSpacing = 80f;
        private const float AnchorCatchupSpeed = 3.5f;
        private const float SteeringResponsiveness = 6f;
        private const float SlowRadius = 90f;
        private const float FriendlySeparationStrength = 36f;

        private AnimatedSprite _animatedSprite;

        private FriendlyState _currentState;

        private enum FriendlyState
        {
            Idle,
            Run,
            Hit
        }

        public Friendly(FriendlyWeapons friendlyWeapon) : base(
            textureName: "Character-Unknown-Idle", // alleen fallback voor Mob system
            speed: Settings.GetValue(SettingsConst.FRIENDLY.MOVE_SPEED),
            startHealth: Settings.GetValue(SettingsConst.FRIENDLY.START_HEALTH),
            maxHealth: Settings.GetValue(SettingsConst.FRIENDLY.MAX_HEALTH),
            frameCount: 6,
            frameRate: 6f,
            scale: 2f
        )
        {
            _radius = Settings.GetValue(SettingsConst.FRIENDLY.RADIUS);
            _velocity = Vector2.Zero;

            switch (friendlyWeapon)
            {
                case FriendlyWeapons.HandGun:
                    _weapon = new Handgun();
                    break;
            }
        }

        public Friendly(FriendlyWeapons friendlyWeapons, Vector2 position) : this(friendlyWeapons)
        {
            _startPosition = position;
            _angleOffset = (float)(GameManager.GetGameManager().RNG.NextDouble() * MathHelper.TwoPi);

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

            base.Load(content);

            _collider.Center = _startPosition;
        }

        private void SwitchAnimation(string name, int frames, float fps, bool loop)
        {
            var texture = GameManager.GetGameManager().Content.Load<Texture2D>(name);

            int frameWidth = texture.Width / frames;

            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _previousCenter = _collider.Center;

            _animatedSprite.Update(gameTime);

            var gameManager = GameManager.GetGameManager();
            Vector2 playerPosition = gameManager.Player.GetPosition().Center.ToVector2();

            int index = gameManager.Friendlies.IndexOf(this);
            int ringSize = 6;
            float baseRadius = 130f;
            float ringSpacing = 80f;

            int ringNumber = index / ringSize;
            int indexInRing = index % ringSize;

            float orbitRadius = baseRadius + ringNumber * ringSpacing;
            float angleStep = MathHelper.TwoPi / ringSize;

            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            float angle = angleStep * indexInRing + time + _angleOffset;

            Vector2 formationTarget = GetFormationTarget(gameManager, _formationAnchor);
            formationTarget = KeepPositionOutsidePlayer(gameManager.Player, formationTarget, _radius + 18f);
            formationTarget += GetFriendlySeparationOffset(gameManager);

            Vector2 correctedPosition = KeepPositionOutsidePlayer(gameManager.Player, _collider.Center, _radius + 4f);
            if (correctedPosition != _collider.Center)
            {
                if (other == this || other == null || other._collider == null)
                    continue;

                float distance = Vector2.Distance(targetPosition, other._collider.Center);
                float minimalDistance = _radius * 2;

                if (correction.LengthSquared() > 0.0001f)
                {
                    Vector2 correctionNormal = Vector2.Normalize(correction);
                    float velocityIntoPlayer = Vector2.Dot(_velocity, correctionNormal);
                    if (velocityIntoPlayer > 0f)
                    {
                        _velocity -= correctionNormal * velocityIntoPlayer;
                    }
                }
            }

            _collider.Center = Vector2.Lerp(_collider.Center, targetPosition, 5f * deltaTime);

            Attack(gameTime);

            Vector2 movement = _collider.Center - _previousCenter;

            if (movement.LengthSquared() > 0.0001f)
                _facingDirection = Vector2.Normalize(movement);

            UpdateState();

            base.Update(gameTime);
        }

        private void UpdateState()
        {
            if (_currentState == FriendlyState.Hit)
                return;

            if ((_collider.Center - _previousCenter).LengthSquared() > 0.01f)
                SetState(FriendlyState.Run);
            else
                SetState(FriendlyState.Idle);
        }

        private void SetState(FriendlyState newState)
        {
            if (_currentState == newState)
                return;

            _currentState = newState;

            switch (newState)
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
            Color tint = Color.LightBlue;

            Rectangle destinationRectangle = GetAnimatedSpriteDestinationRectangle();

            DrawShadow(spriteBatch, destinationRectangle);

            //_animatedSprite.Draw(spriteBatch, tint);
            DrawAnimatedSprite(spriteBatch, Color.LightBlue, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        private Vector2 GetFormationTarget(GameManager gameManager, Vector2 anchorPosition)
        {
            int index = gameManager.Friendlies.IndexOf(this);
            if (index < 0)
            {
                return anchorPosition;
            }

            int ringNumber = index / RingSize;
            int indexInRing = index % RingSize;
            float angleStep = MathHelper.TwoPi / RingSize;
            float ringOffset = ringNumber % 2 == 0 ? 0f : angleStep * 0.5f;
            float angle = indexInRing * angleStep + ringOffset - MathHelper.PiOver2;
            float radius = BaseRadius + ringNumber * RingSpacing;

            Vector2 slotOffset = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * radius;

            return anchorPosition + slotOffset;
        }

        private Vector2 KeepPositionOutsidePlayer(Player player, Vector2 position, float padding)
        {
            Rectangle bounds = player.GetPosition();
            float left = bounds.Left - padding;
            float right = bounds.Right + padding;
            float top = bounds.Top - padding;
            float bottom = bounds.Bottom + padding;

            if (position.X < left || position.X > right || position.Y < top || position.Y > bottom)
            {
                return position;
            }

            float distanceToLeft = position.X - left;
            float distanceToRight = right - position.X;
            float distanceToTop = position.Y - top;
            float distanceToBottom = bottom - position.Y;

            if (distanceToLeft <= distanceToRight && distanceToLeft <= distanceToTop && distanceToLeft <= distanceToBottom)
            {
                position.X = left;
            }
            else if (distanceToRight <= distanceToTop && distanceToRight <= distanceToBottom)
            {
                position.X = right;
            }
            else if (distanceToTop <= distanceToBottom)
            {
                position.Y = top;
            }
            else
            {
                position.Y = bottom;
            }

            return position;
        }

        private Vector2 GetFriendlySeparationOffset(GameManager gameManager)
        {
            Vector2 totalOffset = Vector2.Zero;
            float desiredDistance = _radius * 2.15f;

            foreach (var other in gameManager.Friendlies)
            {
                if (other == this)
                    continue;

                Vector2 away = _collider.Center - other._collider.Center;
                if (away.LengthSquared() < 0.0001f)
                {
                    continue;
                }

                float distance = away.Length();
                if (distance >= desiredDistance)
                {
                    continue;
                }

                away /= distance;
                float strength = 1f - MathHelper.Clamp(distance / desiredDistance, 0f, 1f);
                totalOffset += away * (strength * FriendlySeparationStrength);
            }

            return totalOffset;
        }

        private void MoveTowards(Vector2 targetPosition, float deltaTime)
        {
            Vector2 toTarget = targetPosition - _collider.Center;
            float distance = toTarget.Length();
            float maxSpeed = _speed;
            Vector2 desiredVelocity = Vector2.Zero;

            if (distance > 0.5f)
            {
                Vector2 direction = toTarget / distance;
                float speedFactor = MathHelper.Clamp(distance / SlowRadius, 0f, 1f);
                desiredVelocity = direction * (maxSpeed * speedFactor);
            }

            float steeringBlend = MathHelper.Clamp(SteeringResponsiveness * deltaTime, 0f, 1f);
            _velocity = Vector2.Lerp(_velocity, desiredVelocity, steeringBlend);

            if (_velocity.LengthSquared() > maxSpeed * maxSpeed)
            {
                _velocity = Vector2.Normalize(_velocity) * maxSpeed;
            }

            _collider.Center += _velocity * deltaTime;

            if (distance < 2f && _velocity.LengthSquared() < 9f)
            {
                _velocity = Vector2.Zero;
            }
        }

        private void Attack(GameTime gameTime)
        {
            if (_weapon.CanFire)
            {
                Vector2 nearestZombiePosition = GetNearestZombiePosition();

                Vector2 aimDirection = LinePieceCollider.GetDirection(
                    _collider.Center,
                    nearestZombiePosition
                );

                float distance = Vector2.Distance(nearestZombiePosition, _collider.Center);

                if (distance < 300f)
                {
                    Vector2 targetPosition = nearestEnemy._collider.Center;
                    Vector2 aimDirection = LinePieceCollider.GetDirection(_collider.Center, targetPosition);
                    float distance = Vector2.Distance(targetPosition, _collider.Center);

                    if (distance < 300f)
                    {
                        _weapon.Fire(_collider.Center, aimDirection);
                    }
                }
            }

            _weapon.UpdateCoolDown(gameTime);
        }

        private Mob GetNearestEnemyPosition()
        {
            var enemies = GameManager.GetGameManager().Enemies;

            Mob closest = null;
            float closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                float distance = Vector2.Distance(zombie._collider.Center, _collider.Center);

                var enemyLocation = enemy._collider.Center;
                var distance = Vector2.Distance(enemyLocation, _collider.Center);
                
                if (distance < closestDistance)
                {
                    closest = enemy;
                    closestDistance = distance;
                }
            }

            return closest == null
                ? Vector2.Zero
                : closest._collider.Center;
        }
    }
}
