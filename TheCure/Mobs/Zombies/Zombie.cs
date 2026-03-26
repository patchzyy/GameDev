using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;

namespace TheCure
{
    internal class Zombie : Mob
    {
        public Zombie() : base("Alien", 60f, 3, 5)
        {
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            SetHealthBar(_texture, _maxHealth, _startHealth, BecomeFriendly, null);

            RandomMove();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();
            Vector2 direction = playerPosition - _collider.Center;
            float distance = Vector2.Distance(_collider.Center, playerPosition);

            direction.Normalize();
            _collider.Center += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (distance < 40)
            {
                GameManager.GetGameManager().Player.TakeDamage(20f);
                RandomMove();
            }

            base.Update(gameTime);
        }

        private void BecomeFriendly()
        {
            GameManager gm = GameManager.GetGameManager();
            
            // turn into friendly at same position
            gm.AddGameObject(new Friendly(_collider.Center));
            gm.RemoveGameObject(this);
        }

        public override void OnCollision(GameObject tmp)
        {
            if ((tmp is Bullet || tmp is Laser))
            {
                LoseHealth(1);
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