using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Managers;

namespace TheCure
{
    public class Button
    {
        public Rectangle Rectangle { get; private set; }
        public string Text { get; private set; }

        private SpriteFont _font;
        private bool _isHovering;
        public event Action Action;

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

        public void SetAction(Action action)
        {
            Action = action;
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
                Action?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var dummyTexture = ContentsManager.Get().DummyTexture;
            Color bgColor = _isHovering ? new Color(100, 200, 100, 255) : new Color(70, 150, 70, 255);
            spriteBatch.Draw(dummyTexture, Rectangle, bgColor);

            Color borderColor = _isHovering ? new Color(255, 200, 0, 255) : new Color(255, 255, 255, 150);
            int borderThickness = 2;

            spriteBatch.Draw(dummyTexture,
                new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, borderThickness), borderColor);
            spriteBatch.Draw(dummyTexture,
                new Rectangle(Rectangle.X, Rectangle.Y + Rectangle.Height - borderThickness, Rectangle.Width,
                    borderThickness), borderColor);
            spriteBatch.Draw(dummyTexture,
                new Rectangle(Rectangle.X, Rectangle.Y, borderThickness, Rectangle.Height), borderColor);
            spriteBatch.Draw(dummyTexture,
                new Rectangle(Rectangle.X + Rectangle.Width - borderThickness, Rectangle.Y, borderThickness,
                    Rectangle.Height), borderColor);

            Vector2 size = _font.MeasureString(Text);
            Vector2 position = new Vector2(Rectangle.X + (Rectangle.Width - size.X) / 2,
                Rectangle.Y + (Rectangle.Height - size.Y) / 2);

            Color textColor = _isHovering ? Color.Yellow : Color.White;
            spriteBatch.DrawString(_font, Text, position, textColor);
        }
    }
}