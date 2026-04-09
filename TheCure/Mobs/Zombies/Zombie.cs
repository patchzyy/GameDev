using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    public class Zombie : Mob
    {
        // constants
        private float _attackCooldown;
        private float _stagger;
        private int _attackDamage;

        // states
        private bool _attackNextCombat;
        private float _attackTimer;
        private GameObject _currentTarget;
        private Vector2 _previousCenter;

        public float LastHealed;

        public Zombie() : base(
            "Alien",
            Settings.GetValue(SettingsConst.ZOMBIE.SPEED),
            Settings.GetValue(SettingsConst.ZOMBIE.START_HEALTH),
            Settings.GetValue(SettingsConst.ZOMBIE.MAX_HEALTH))
        {
            _stagger = Settings.GetValue(SettingsConst.ZOMBIE.STAGGER);
            _attackDamage = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_DAMAGE);
            _attackCooldown = Settings.GetValue(SettingsConst.ZOMBIE.ATTACK_COOLDOWN);
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
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), tint);

            base.Draw(gameTime, spriteBatch);
        }
    }
}