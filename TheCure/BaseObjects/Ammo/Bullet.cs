using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.World;

namespace TheCure
{
    internal class Bullet : GameObject
    {
        private Texture2D _texture;
        private CircleCollider _collider;
        private Vector2 _velo;

        public bool IsHealing = false;
        public float Damage { get; }

        private float _life = 3.0f;

        public Bullet(Vector2 location, Vector2 direction, float speed, bool isHealing = false, float damage = 1f)
        {
            _collider = new CircleCollider(location, 4);
            SetCollider(_collider);
            _velo = direction * speed;
            IsHealing = isHealing;
            Damage = damage;
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

        public override void OnCollision(GameObject gameObject)
        {
            if (gameObject is Zombie)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }
            else if (gameObject is Supply)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), Color.Red);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
