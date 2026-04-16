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

        public Friendly(FriendlyWeapons friendlyWeapon) : base(
            textureName: "player",
            speed: Settings.GetValue(SettingsConst.FRIENDLY.MOVE_SPEED),
            startHealth: Settings.GetValue(SettingsConst.FRIENDLY.START_HEALTH),
            maxHealth: Settings.GetValue(SettingsConst.FRIENDLY.MAX_HEALTH),
            frameCount: 5,
            frameRate: 5f,
            scale: 0.35f
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
            _formationAnchor = position;
            _hasFormationAnchor = true;
            GameManager.GetGameManager().Friendlies.Add(this);
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _collider.Center = _startPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var gameManager = GameManager.GetGameManager();
            Vector2 playerPosition = gameManager.Player.GetPosition().Center.ToVector2();
            _previousCenter = _collider.Center;

            if (gameManager.Friendlies.Count == 0)
                return;

            if (!_hasFormationAnchor)
            {
                _formationAnchor = playerPosition;
                _hasFormationAnchor = true;
            }

            float anchorBlend = MathHelper.Clamp(AnchorCatchupSpeed * deltaTime, 0f, 1f);
            _formationAnchor = Vector2.Lerp(_formationAnchor, playerPosition, anchorBlend);

            Vector2 formationTarget = GetFormationTarget(gameManager, _formationAnchor);
            formationTarget = KeepPositionOutsidePlayer(gameManager.Player, formationTarget, _radius + 18f);
            formationTarget += GetFriendlySeparationOffset(gameManager);

            Vector2 correctedPosition = KeepPositionOutsidePlayer(gameManager.Player, _collider.Center, _radius + 4f);
            if (correctedPosition != _collider.Center)
            {
                Vector2 correction = correctedPosition - _collider.Center;
                _collider.Center = correctedPosition;

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

            MoveTowards(formationTarget, deltaTime);
            Attack(gameTime);

            Vector2 movement = _collider.Center - _previousCenter;
            if (movement.LengthSquared() > 0.0001f)
            {
                _facingDirection = Vector2.Normalize(movement);
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet && tmp is Bullet bullet && bullet.IsHealing)
            {
                if (!_healthBar.IsMaxHealth)
                {
                    GainHealth(1);
                    tmp.Destroy();
                }
            }

            if (tmp is Wall wall)
            {
                Vector2 collisionNormal = wall.ResolveCircleCollision(_collider, _previousCenter);
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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.LightBlue;
            Rectangle destinationRectangle = GetAnimatedSpriteDestinationRectangle();
            DrawShadow(spriteBatch, destinationRectangle);
            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }

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
                Mob nearestEnemy = GetNearestEnemyPosition();
                if (nearestEnemy != null)
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

                if (enemy is Zombie zombie && zombie.LastHealed < 3f) continue;

                var enemyLocation = enemy._collider.Center;
                var distance = Vector2.Distance(enemyLocation, _collider.Center);
                
                if (distance < closestDistance)
                {
                    closest = enemy;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}
