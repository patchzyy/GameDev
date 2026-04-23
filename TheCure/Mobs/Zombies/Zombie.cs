using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    public class Zombie : Mob
    {
        // constants
        private float _attackCoolDown;
        private float _stagger;
        private float _attackDamage;

        // states
        private bool _attackNextCombat;
        private float _attackTimer;
        private GameObject _currentTarget;
        private Vector2 _previousCenter;
        private Vector2 _facingDirection = Vector2.UnitX;


        public Zombie() : base(
            textureName: "Zombie",
            speed: Settings.GetValue(SettingsConst.ZOMBIE.SPEED),
            startHealth: Settings.GetValue(SettingsConst.ZOMBIE.START_HEALTH),
            maxHealth: Settings.GetValue(SettingsConst.ZOMBIE.MAX_HEALTH),
            frameCount: 5,
            frameRate: 5f,
            scale: 0.35f
        )
        {
            _stagger = Settings.GetValue(SettingsConst.ZOMBIE.STAGGER);
            _attackDamage = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_DAMAGE);
            _attackCoolDown = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_COOL_DOWN);
        }

        public override void Load()
        {
            base.Load();

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, BecomeFriendly);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _previousCenter = _collider.Center;

            UpdateKnockBack(deltaTime);

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
                ? PlayerManager.Get().Player.GetPosition().Center.ToVector2()
                : _currentTarget.GetCollider().GetBoundingBox().Center.ToVector2();
            Vector2 direction = targetPosition - _collider.Center;

            direction.Normalize();
            _collider.Center += direction * (_speed / 2f) * deltaTime;
        }

        private void BecomeFriendly()
        {
            GameManager gameManager = GameManager.Get();

            // turn into friendly at same position
            gameManager.AddGameObject(new Friendly(FriendlyWeapons.HandGun, _collider.Center));
            gameManager.RemoveGameObject(this);

            ScoreManager.Get().AddScore(100, "Zombie Healed"); // Add score for converting a zombie to friendly
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
            _attackTimer = _attackCoolDown;
            _currentTarget = null;
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet bullet)
            {
                if (bullet.IsHealing)
                {
                    GainHealth(1);
                    LastHealed = 0f;
                }
                else
                {
                    LoseHealth(bullet.Damage);
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
            ScoreManager.Get().AddScore(50, "Zombie Killed");
            base.Destroy();
        }

        public void RandomMove()
        {
            var gameManager = GameManager.Get();
            _collider.Center = gameManager.RandomLocationOutsideView((int)_collider.Radius);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.White;
            Rectangle destRect = GetAnimatedSpriteDestinationRectangle();
            DrawShadow(spriteBatch, destRect);
            DrawAnimatedSprite(spriteBatch, tint, _facingDirection);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
