using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Collision;

namespace TheCure.World
{
    public class Supply : GameObject
    {
        private CircleCollider _collider;
        private Texture2D _texture;

        public Supply()
        {
            _collider = new CircleCollider(Vector2.Zero, 15);
            SetCollider(_collider);
        }

        public override void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>("player");
            base.Load(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other)
        {
            // Supply doesn't destroy itself on collision, bullets pass through
        }

        public void RandomMove()
        {
            var game = GameManager.GetGameManager();
            _collider.Center = game.RandomLocationOutsideView((int)_collider.Radius);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                var boundingBox = _collider.GetBoundingBox();
                spriteBatch.Draw(_texture, boundingBox, Color.White);
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}
