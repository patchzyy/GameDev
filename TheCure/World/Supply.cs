using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;

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

        public override void Load()
        {
            var content = ContentsManager.Get().GetContent();
            _texture = content.Load<Texture2D>("player");
            base.Load();
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
            var game = GameManager.Get();
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
