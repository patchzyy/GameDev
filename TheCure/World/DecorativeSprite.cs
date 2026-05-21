using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;

namespace TheCure
{
    internal class DecorativeSprite : GameObject
    {
        private readonly string _debugName;
        private readonly string _textureAssetName;
        private readonly Rectangle _sourceRectangle;
        private readonly Rectangle _destinationRectangle;
        private readonly float _rotation;
        private Texture2D _texture;

        public DecorativeSprite(
            string debugName,
            string textureAssetName,
            Rectangle sourceRectangle,
            Rectangle destinationRectangle,
            float rotation = 0f)
        {
            _debugName = debugName;
            _textureAssetName = textureAssetName;
            _sourceRectangle = sourceRectangle;
            _destinationRectangle = destinationRectangle;
            _rotation = rotation;
        }

        public override void Load()
        {
            _texture = ContentsManager.Get().GetContent().Load<Texture2D>(_textureAssetName);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null)
                return;

            if (_rotation == 0f)
            {
                spriteBatch.Draw(_texture, _destinationRectangle, _sourceRectangle, Color.White);
                return;
            }

            Vector2 scale = new(
                _destinationRectangle.Width / (float)_sourceRectangle.Width,
                _destinationRectangle.Height / (float)_sourceRectangle.Height);

            spriteBatch.Draw(
                _texture,
                new Vector2(_destinationRectangle.Center.X, _destinationRectangle.Center.Y),
                _sourceRectangle,
                Color.White,
                _rotation,
                new Vector2(_sourceRectangle.Width / 2f, _sourceRectangle.Height / 2f),
                scale,
                SpriteEffects.None,
                0f);
        }

        public override string ToString()
        {
            return _debugName;
        }
    }
}
