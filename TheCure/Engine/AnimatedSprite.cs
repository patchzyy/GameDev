using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    public class AnimatedSprite
    {
        private readonly Texture2D _texture;
        private readonly int _columns;
        private float _frameTimer;

        //tip btw if you have a sprite sheet with 1 row, you can do frameHeight = texture.Height and frameWidth = texture.Width / frameCount
        public int FrameWidth { get; } //This should be the width of 1 frame
        public int FrameHeight { get; } //This should be the height of 1 frame

        public int FrameCount { get; }
        public int CurrentFrame { get; private set; }
        public float FrameRate { get; set; }
        public bool IsLooping { get; set; }
        public bool IsPlaying { get; private set; }
        public bool IsReversed { get; set; }

        public Point FrameSize => new(FrameWidth, FrameHeight);
        public Vector2 Origin => new(FrameWidth / 2f, FrameHeight / 2f);
        private float SecondsPerFrame => 1f / FrameRate;
        public bool IsFinished => !IsLooping && (
            (!IsReversed && CurrentFrame == FrameCount - 1 && !IsPlaying) || 
            (IsReversed && CurrentFrame == 0 && !IsPlaying));

        public Rectangle SourceRectangle
        {
            get
            {
                int column = CurrentFrame % _columns;
                int row = CurrentFrame / _columns;

                return new Rectangle(column * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
            }
        }

        public AnimatedSprite(Texture2D texture, int frameWidth, int frameHeight, int frameCount, float frameRate, bool isLooping = true, bool isReversed = false)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            if (frameWidth <= 0 || frameHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameWidth), "Frame size must be greater than zero.");

            if (frameRate <= 0f)
                throw new ArgumentOutOfRangeException(nameof(frameRate), "Frame rate must be greater than zero.");

            if (frameCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameCount), "Frame count must be greater than zero.");

            if (frameWidth > texture.Width || frameHeight > texture.Height)
                throw new ArgumentException($"Frame size ({frameWidth}x{frameHeight}) cannot be larger than texture size ({texture.Width}x{texture.Height}).");

            _columns = texture.Width / frameWidth;
            int rows = texture.Height / frameHeight;
            int maxFrameCount = _columns * rows;

            if (frameCount > maxFrameCount)
                throw new ArgumentOutOfRangeException(nameof(frameCount), $"Frame count ({frameCount}) exceeds maximum possible frames ({maxFrameCount}) for {_columns}x{rows} grid.");

            _texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCount = frameCount;
            FrameRate = frameRate;
            IsLooping = isLooping;
            IsPlaying = true;
            IsReversed = isReversed;
            CurrentFrame = isReversed ? frameCount - 1 : 0;
            _frameTimer = SecondsPerFrame;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPlaying || FrameCount == 1)
                return;

            _frameTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // A long frame can skip ahead more than once, so keep advancing until caught up.
            while (_frameTimer <= 0f)
            {
                AdvanceFrame();

                if (!IsPlaying)
                    break;

                _frameTimer += SecondsPerFrame;
            }
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void Reset()
        {
            CurrentFrame = IsReversed ? FrameCount - 1 : 0;
            _frameTimer = SecondsPerFrame;
            IsPlaying = true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation = 0f,
            float scale = 1f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(_texture, position, SourceRectangle, color, rotation, Origin, scale, effects, layerDepth);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(_texture, destinationRectangle, SourceRectangle, color);
        }

        private void AdvanceFrame()
        {
            if (IsReversed)
            {
                if (CurrentFrame > 0)
                {
                    CurrentFrame--;
                    return;
                }

                if (IsLooping)
                {
                    CurrentFrame = FrameCount - 1;
                    return;
                }

                CurrentFrame = 0;
                IsPlaying = false;
                return;
            }

            if (CurrentFrame < FrameCount - 1)
            {
                CurrentFrame++;
                return;
            }

            if (IsLooping)
            {
                CurrentFrame = 0;
                return;
            }

            CurrentFrame = FrameCount - 1;
            IsPlaying = false;
        }
    }
}
