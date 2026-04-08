using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    public class Zombie : Mob
    {
        private bool _attackNextCombat;
        private float _attackCooldown = 1f;
        private float _attackTimer;
        private GameObject _currentTarget;
        private float _stagger = 1f;
        private int _attackDamage = 1;
        private Vector2 _previousCenter;
        private Vector2 _facingDirection = Vector2.UnitX;

        public float LastHealed;

        public Zombie() : base("Zombie", 60f, 3, 10, frameCount: 5, frameRate: 5f, scale: 0.35f)
        {
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, BecomeFriendly);

            RandomMove();
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

            LastHealed += deltaTime;

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

            direction.Normalize();
            _collider.Center += direction * (_speed / 2f) * deltaTime;
        }

        private void BecomeFriendly()
        {
            GameManager gm = GameManager.GetGameManager();

            // turn into friendly at same position
            gm.AddGameObject(new Friendly(FriendlyWeapons.HandGun, _collider.Center));
            gm.RemoveGameObject(this);

            gm.AddScore(100, "Zombie Healed"); // Add score for converting a zombie to friendly
        }

        private void Attack(float deltaTime)
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= deltaTime;
                return;
            }

            _currentTarget.LoseHealth(_attackDamage);
            _attackNextCombat = false;
            _attackTimer = _attackCooldown;
            _currentTarget = null;
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet)
            {
                if ((tmp as Bullet).IsHealing)
                {
                    GainHealth(1);
                    LastHealed = 0f;
                }
                else
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
            GameManager.GetGameManager().AddScore(50, "Zombie Killed");
            base.Destroy();
        }

        public void RandomMove()
        {
            var game = GameManager.GetGameManager();
            _collider.Center = game.RandomLocationOutsideView((int)_collider.Radius);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.White;
            int scaledWidth = (int)(_animatedSprite.FrameWidth * 0.35f);
            int scaledHeight = (int)(_animatedSprite.FrameHeight * 0.35f);

            Rectangle destRect = new Rectangle(
                (int)(_collider.Center.X - scaledWidth / 2),
                (int)(_collider.Center.Y - scaledHeight / 2),
                scaledWidth,
                scaledHeight
            );

            Rectangle shadowCore = new Rectangle(
                destRect.X + destRect.Width / 8,
                destRect.Y + destRect.Height - 6,
                destRect.Width - destRect.Width / 4,
                4
            );
            Rectangle shadowSoft = new Rectangle(
                destRect.X + destRect.Width / 6,
                destRect.Y + destRect.Height - 4,
                destRect.Width - destRect.Width / 3,
                2
            );

            spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, shadowCore, Color.Black * 0.14f);
            spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, shadowSoft, Color.Black * 0.08f);

            SpriteEffects effects = _facingDirection.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(_texture, destRect, _animatedSprite.SourceRectangle, tint, 0f, Vector2.Zero, effects, 0f);

            base.Draw(gameTime, spriteBatch);
        }
    }
}