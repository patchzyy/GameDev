using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheCure
{
    public enum PlanetType
    {
        Pickup,
        DropOff
    }

    internal class Planet : GameObject
    {
        private Texture2D _spriteSheet;
        private CircleCollider _circleCollider;
        private Vector2 _initialPosition;

        public PlanetType Type
        {
            get;
            private set;
        }

        private int _frameCount;
        private int _frameWidth;
        private int _frameHeight;

        public Planet(Vector2 position, PlanetType type)
        {
            _initialPosition = position;
            Type = type;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            string textureName = (Type == PlanetType.Pickup) ? "PlanetPickup" : "PlanetDropoff";

            try
            {
                _spriteSheet = content.Load<Texture2D>(textureName);
            }
            catch (ContentLoadException)
            {
                System.Diagnostics.Debug.WriteLine($"Waarschuwing: Kon textuur '{textureName}' niet laden. Laad standaard 'Alien' textuur in plaats daarvan.");
                _spriteSheet = content.Load<Texture2D>("Alien");
            }

            if (_spriteSheet != null)
            {
                _frameCount = 60;

                if (_frameCount > 0)
                {
                    _frameHeight = _spriteSheet.Height;
                    _frameWidth = _spriteSheet.Width / _frameCount;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Waarschuwing: Ongeldig frame aantal ({_frameCount}) voor planeet {Type}. Gebruik volledige textuur afmetingen.");

                    _frameHeight = _spriteSheet.Height;
                    _frameWidth = _spriteSheet.Width;
                    _frameCount = 1;
                }

                float radius = Math.Max(_frameWidth, _frameHeight) / 2f * 0.9f;

                _circleCollider = new CircleCollider(_initialPosition, radius);

                SetCollider(_circleCollider);

                System.Diagnostics.Debug.WriteLine($"Statische Frame Planeet {Type} geladen. Frame B: {_frameWidth}, Frame H: {_frameHeight}. Collider bij {_circleCollider.Center}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Fout: Kon geen textuur laden voor Planeet bij {_initialPosition}. Kan afmetingen of collider niet instellen.");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_spriteSheet != null && collider is CircleCollider circleCollider && _frameWidth > 0 && _frameHeight > 0)
            {
                int adjustment = 15;
                int Width = _frameWidth - adjustment;

                if (Width < 1)
                {
                    Width = 1;
                }

                Rectangle rectangle = new Rectangle(0, 0, Width, _frameHeight);
                Vector2 origin = new Vector2(_frameWidth / 2f, _frameHeight / 2f);

                spriteBatch.Draw(
                    _spriteSheet,
                    circleCollider.Center,
                    rectangle,
                    Color.White,
                    0f,
                    origin,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}