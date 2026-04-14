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
        private float _followDistance;
        private Vector2 _startPosition;
        private float _angleOffset;
        private float _radius;
        private Vector2 _previousCenter;
        private Vector2 _facingDirection = Vector2.UnitX;

        private BaseWeapon _weapon;

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
            _followDistance = Settings.GetValue(SettingsConst.FRIENDLY.FOLLOW_DISTANCE);
            _radius = Settings.GetValue(SettingsConst.FRIENDLY.RADIUS);

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

            Vector2 offset = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * orbitRadius;

            Vector2 targetPosition = playerPosition + offset;

            foreach (var other in gameManager.Friendlies)
            {
                if (other == this || other == null || other._collider == null)
                    continue;

                float distance = Vector2.Distance(targetPosition, other._collider.Center);
                float minimalDistance = _radius * 2;

                if (distance < minimalDistance && distance > 0)
                {
                    Vector2 push = targetPosition - other._collider.Center;
                    push.Normalize();
                    targetPosition += push * (minimalDistance - distance);
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
                if (zombie.LastHealed < 3f)
                    continue;

                float distance = Vector2.Distance(zombie._collider.Center, _collider.Center);

                if (distance < closestDistance)
                {
                    closest = zombie;
                    closestDistance = distance;
                }
            }

            return closest == null
                ? Vector2.Zero
                : closest._collider.Center;
        }
    }
}