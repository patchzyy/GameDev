using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    public class Camera
    {
        private Vector2 _position;
        private readonly GraphicsDevice _graphicsDevice;

        public Camera(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _position = Vector2.Zero;
        }

        private Viewport Viewport => _graphicsDevice.Viewport;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public void Update(Vector2 target)
        {
            _position.X = target.X - Viewport.Width / 2;
            _position.Y = target.Y - Viewport.Height / 2;
        }

        public void Update(Vector2 target, Rectangle worldBounds)
        {
            Update(target);

            // cap camera
            int maxX = worldBounds.Right - Viewport.Width;
            int maxY = worldBounds.Bottom - Viewport.Height;

            if (maxX < worldBounds.Left)
            {
                _position.X = worldBounds.Left;
            }
            else
            {
                _position.X = MathHelper.Clamp(_position.X, worldBounds.Left, maxX);
            }

            if (maxY < worldBounds.Top)
            {
                _position.Y = worldBounds.Top;
            }
            else
            {
                _position.Y = MathHelper.Clamp(_position.Y, worldBounds.Top, maxY);
            }
        }

        public Rectangle GetViewBounds()
        {
            return new Rectangle((int)_position.X, (int)_position.Y, Viewport.Width, Viewport.Height);
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-_position, 0f));
        }
    }
}