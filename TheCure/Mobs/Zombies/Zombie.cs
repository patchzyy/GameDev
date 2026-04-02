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

        public float LastHealed;

        public Zombie() : base("Alien", 60f, 3, 10)
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

            base.OnCollision(tmp);
        }

        public void RandomMove()
        {
            GameManager game = GameManager.GetGameManager();
            _collider.Center = game.RandomLocationOutsideView();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.White;
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), tint);

            base.Draw(gameTime, spriteBatch);
        }
    }
}