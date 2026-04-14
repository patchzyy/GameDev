using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;

namespace TheCure
{
    public class Brute : Mob
    {
        private float _attackCooldown;
        private float _stagger;
        private int _attackDamage;

        private bool _attackNextCombat;
        private float _attackTimer;
        private GameObject _currentTarget;
        private Vector2 _previousCenter;
        private Vector2 _facingDirection = Vector2.UnitX;

        public Brute() : base(
            textureName: "Zombie",
            speed: 30f,
            startHealth: 15f,
            maxHealth: 15f,
            frameCount: 5,
            frameRate: 2.5f,
            scale: 0.55f
        )
        {
            _stagger = 1.2f;
            _attackDamage = 3;
            _attackCooldown = 2f;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            // no BecomeFriendly here, brute cannot be healed
            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousCenter = _collider.Center;

            if (_attackNextCombat)
            {
                Attack(deltaTime);
            }
            else
            {
                Move(deltaTime);
            }

            Vector2 movement = _collider.Center - _previousCenter;
            if (movement.LengthSquared() > 0.0001f)
            {
                _facingDirection = Vector2.Normalize(movement);
            }

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

            if (direction.LengthSquared() > 0.0001f)
            {
                direction.Normalize();
                _collider.Center += direction * (_speed / 2f) * deltaTime;
            }
        }

        private void Attack(float deltaTime)
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= deltaTime;
                return;
            }

            if (_currentTarget != null)
            {
                _currentTarget.LoseHealth(_attackDamage);
            }

            _attackNextCombat = false;
            _attackTimer = _attackCooldown;
            _currentTarget = null;
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet bullet)
            {
                // brute ignores healing bullets
                if (!bullet.IsHealing)
                {
                    LoseHealth(1);
                }

                tmp.Destroy();
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

        public override void Destroy()
        {
            GameManager.GetGameManager().AddScore(75, "Brute Killed");
            base.Destroy();
        }

        public void RandomMove()
        {
            var game = GameManager.GetGameManager();
            _collider.Center = game.RandomLocationOutsideView((int)_collider.Radius);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle destRect = GetAnimatedSpriteDestinationRectangle();
            DrawShadow(spriteBatch, destRect, 0.18f, 0.10f);
            DrawAnimatedSprite(spriteBatch, Color.White, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }
    }
}