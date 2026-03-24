using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    public class Camera
    {
        private Vector2 _position;
        private Viewport _viewport;

        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            _position = Vector2.Zero;
        }

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
            _position.X = target.X - _viewport.Width / 2;
            _position.Y = target.Y - _viewport.Height / 2;
        }

        public Rectangle GetViewBounds()
        {
            return new Rectangle((int)_position.X, (int)_position.Y, _viewport.Width, _viewport.Height);
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-_position, 0f));
        }
    }
}