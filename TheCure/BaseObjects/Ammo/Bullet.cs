using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    internal class Bullet : GameObject
    {
        private Texture2D _texture;
        private CircleCollider _collider;
        private Vector2 _velo;

        public bool IsHealing = false;

        private float _life = 3.0f;

        public Bullet(Vector2 location, Vector2 direction, float speed, bool isHealing = false)
        {
            _collider = new CircleCollider(location, 4);
            SetCollider(_collider);
            _velo = direction * speed;
            IsHealing = isHealing;
        }

        public override void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Bullet");
            base.Load(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _collider.Center += _velo * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _life -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_life <= 0)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Zombie)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }
            else if (other is Supply)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), Color.Red);
            base.Draw(time, spriteBatch);
        }
    }
}