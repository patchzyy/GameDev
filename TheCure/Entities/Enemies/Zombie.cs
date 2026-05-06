using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    public class Zombie : Enemy
    {
        private Vector2 _spawnPosition;

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
            _attackCooldown = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_COOL_DOWN);
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            _collider.Center = _spawnPosition;
            _state = EnemyState.Spawning;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, BecomeFriendly);
            SyncHealthBarPosition();

            SwitchAnimation("Zombie-Dead", 11, 5f, false, true);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateAnimation();
            base.Update(gameTime);
        }

        protected override void Move(float dt)
        {
            if (_staggerTimer > 0f)
                return;

            Vector2 target = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();

            Vector2 dir = target - _collider.Center;

            if (dir.LengthSquared() > 0.01f)
                dir.Normalize();

            _collider.Center += dir * (_speed * 0.5f) * dt;
        }

        private void BecomeFriendly()
        {
            if (_state == EnemyState.Dying)
                return;

            _state = EnemyState.Dying;

            SwitchAnimation("Zombie-Dead", 11, 5f, false);

            Vector2 spawnPosition = _collider.Center;

            _onDeathComplete = () =>
            {
                var gm = GameManager.GetGameManager();
                gm.AddGameObject(new Friendly(FriendlyWeapons.HandGun, spawnPosition));
                gm.RemoveGameObject(this);
                gm.AddScore(100, "Zombie Healed");
            };
        }

        public override void Destroy()
        {
            if (_state == EnemyState.Dying)
                return;

            _state = EnemyState.Dying;

            SwitchAnimation("Zombie-Dead", 11, 5f, false);

            _onDeathComplete = null; 
        }

        

        private void UpdateAnimation()
        {
            if (_state == EnemyState.Dying || _state == EnemyState.Spawning)
                return;

            if (_state == EnemyState.Attack)
                if (_currentAnimation != "Zombie-Atk")
                    {
                        SwitchAnimation("Zombie-Atk", 7, 5f, true);
                        _currentAnimation = "Zombie-Atk";
                    }
            else if (_state == EnemyState.Walk) 
            {
                if (_currentAnimation != "Zombie-Walk")
                {
                    SwitchAnimation("Zombie-Walk", 7, 3.5f, true);
                    _currentAnimation = "Zombie-Walk";
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = _isFlashing ? _flashColor : Color.White;
            Rectangle destRect = GetAnimatedSpriteDestinationRectangle();

            DrawShadow(spriteBatch, destRect);

            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }

        public void Spawn(Vector2 position)
        {
            _spawnPosition = position;
        }
    }
}
