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
        private float _followDistance = 60f;
        private Vector2 _startPosition;
        private float _angleOffset;
        private float _radius = 20f;
        private Vector2 _previousCenter;
        private Vector2 _facingDirection = Vector2.UnitX;

        private BaseWeapon _weapon;

        public Friendly(FriendlyWeapons friendlyWeapon) : base("player", 60f, 3, 10, frameCount: 10, frameRate: 5f, scale: 0.35f)
        {
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
            base.Load(content);
            _collider.Center = _startPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var gm = GameManager.GetGameManager();
            Vector2 playerPosition = gm.Player.GetPosition().Center.ToVector2();
            _previousCenter = _collider.Center;

            int count = gm.Friendlies.Count;
            if (count == 0) return;

            int index = gm.Friendlies.IndexOf(this);


            int ringSize = 6;
            float baseRadius = 130f;
            float ringSpacing = 80f;

            int ringNumber = index / ringSize;
            int indexInRing = index % ringSize;

            float orbitRadius = baseRadius + ringNumber * ringSpacing;

            float angleStep = MathHelper.TwoPi / ringSize;
            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            float angle = angleStep * indexInRing + time + _angleOffset;

            Vector2 offset = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * orbitRadius;

            Vector2 targetPos = playerPosition + offset;

            foreach (var other in gm.Friendlies)
            {
                if (other == this) continue;

                float dist = Vector2.Distance(targetPos, other._collider.Center);
                float minDist = _radius * 2;

                if (dist < minDist && dist > 0)
                {
                    Vector2 push = targetPos - other._collider.Center;
                    push.Normalize();
                    targetPos += push * (minDist - dist);
                }
            }

            _collider.Center = Vector2.Lerp(_collider.Center, targetPos, 5f * deltaTime);
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
                // todo: dit is buggy en ziet er slecht uit maar geen tijd om te fixen nu
                // gebeurd wel alleen bij friendly, misschien omdat ze persee rondje wille maken
                wall.ResolveCircleCollision(_collider, _previousCenter);
            }

            base.OnCollision(tmp);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.LightBlue;
            int scaledWidth = (int)(_animatedSprite.FrameWidth * 0.35f);
            int scaledHeight = (int)(_animatedSprite.FrameHeight * 0.35f);

            Rectangle destRect = new Rectangle(
                (int)(_collider.Center.X - scaledWidth / 2),
                (int)(_collider.Center.Y - scaledHeight / 2),
                scaledWidth,
                scaledHeight
            );

            SpriteEffects effects = _facingDirection.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(_texture, destRect, _animatedSprite.SourceRectangle, tint, 0f, Vector2.Zero, effects, 0f);

            base.Draw(gameTime, spriteBatch);
        }

        private void Move(GameTime gameTime)
        {
            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();
            Vector2 direction = playerPosition - _collider.Center;
            float distance = Vector2.Distance(_collider.Center, playerPosition);

            if (distance > _followDistance)
            {
                direction.Normalize();
                _collider.Center += direction * (_speed + 20f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private void Attack(GameTime gameTime)
        {
            if (_weapon.CanFire)
            {
                Vector2 nearestZombiePosition = GetNearestZombiePosition();
                Vector2 aimDirection =
                    LinePieceCollider.GetDirection(_collider.Center,
                        nearestZombiePosition);

                float distance = Vector2.Distance(nearestZombiePosition, _collider.Center);
                if (distance < 300f)
                {
                    _weapon.Fire(_collider.Center, aimDirection, this);
                }
            }

            _weapon.UpdateCoolDown(gameTime);
        }

        private Vector2 GetNearestZombiePosition()
        {
            var zombies = GameManager.GetGameManager().Zombies;

            Zombie closest = null;
            float closestDistance = float.MaxValue;

            foreach (var zombie in zombies)
            {
                if (zombie.LastHealed < 3f) continue;

                var zombieLocation = zombie._collider.Center;
                var distance = Vector2.Distance(zombieLocation, _collider.Center);
                if (distance < closestDistance)
                {
                    closest = zombie;
                    closestDistance = distance;
                }
            }

            if (closest == null) return Vector2.Zero;

            return closest._collider.Center;
        }
    }
}