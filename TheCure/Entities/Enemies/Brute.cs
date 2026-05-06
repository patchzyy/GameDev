using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;

namespace TheCure
{
    public class Brute : Enemy
    {
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
            _attackCooldown = 2f;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            _collider.Center = _spawnPosition;
            _state = EnemyState.Spawning;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);
            SyncHealthBarPosition();

            SwitchAnimation("Zombie-Dead", 11, 3.5f, false, true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAnimation();
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

        public override void Destroy()
        {
            if (_state == EnemyState.Dying)
                return;

            base.Destroy();

            SwitchAnimation("Zombie-Dead", 11, 3.5f, false);
        }

        private void UpdateAnimation()
        {
            if (_state != EnemyState.Walk)
                return;

            bool attacking = _target != null;

            if (attacking)
                SwitchAnimation("Zombie-Atk", 7, 5f, true);
            else
                SwitchAnimation("Zombie-Walk", 7, 3.5f, true);
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
}