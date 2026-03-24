using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    internal class Alien : GameObject
    {
        private CircleCollider _collider;
        private Texture2D _texture;
        private float _speed = 60f;
        private bool _isGameOver = false;
        private int _maxHealth = 5;
        private int _startHealth = 3;

        public override void Load(ContentManager content)
        {
            base.Load(content);

            _texture = content.Load<Texture2D>("Alien");
            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, Destroy);
            _collider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_collider);

            RandomMove();
        }

        public override void Update(GameTime gameTime)
        {
            if (_isGameOver)
            {
                return;
            }

            Vector2 position = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();
            Vector2 direction = position - _collider.Center;

            direction.Normalize();

            _collider.Center += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            float distance = Vector2.Distance(_collider.Center, position);

            if (distance < 40)
            {
                GameManager.GetGameManager().Player.TakeDamage(20f);
                RandomMove();
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet || tmp is Laser)
            {
                LoseHealth(1);
            }

            base.OnCollision(tmp);
        }

        public void RandomMove()
        {
            GameManager game = GameManager.GetGameManager();
            _collider.Center = game.RandomLocationOutsideView();

            Vector2 centerOfPlayer = game.Player.GetPosition().Center.ToVector2();

            while ((_collider.Center - centerOfPlayer).Length() < 100)
            {
                _collider.Center = game.RandomLocationOutsideView();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), Color.White);

            base.Draw(gameTime, spriteBatch);
        }

        public override void Destroy()
        {
            GameManager.GetGameManager().RemoveGameObject(this);
        }
    }
}