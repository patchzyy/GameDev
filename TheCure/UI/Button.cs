using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheCure
{
    public class Button
    {
        public Rectangle Rectangle
        {
            get;
            private set;
        }
        public string Text
        {
            get;
            private set;
        }
        private SpriteFont _font;
        private bool _isHovering;
        public event EventHandler Clicked;

        public Button(Rectangle rectangle, string text, SpriteFont font)
        {
            Rectangle = rectangle;
            Text = text;
            _font = font;
            _isHovering = false;
        }

        public void SetPosition(int x, int y)
        {
            Rectangle = new Rectangle(x, y, Rectangle.Width, Rectangle.Height);
        }


        public void Update(MouseState mouseState)
        {
            if (Rectangle.Contains(mouseState.X, mouseState.Y))
            {
                _isHovering = true;
            }
            else
            {
                _isHovering = false;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && _isHovering)
            {
                Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color.Green;

            if (_isHovering)
            {
                color = Color.DarkGreen;
            }

            spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, Rectangle, color);

            Vector2 size = _font.MeasureString(Text);
            Vector2 position = new Vector2(Rectangle.X + (Rectangle.Width - size.X) / 2, Rectangle.Y + (Rectangle.Height - size.Y) / 2);

            spriteBatch.DrawString(_font, Text, position, Color.White);
        }
    }
}