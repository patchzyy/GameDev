using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheCure
{
    internal class Asteroid : GameObject
    {
        private Texture2D _texture;
        private CircleCollider _circleCollider;
        private Vector2 _initialPosition;

        public Asteroid(Vector2 position)
        {
            _initialPosition = position;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            try
            {
                _texture = content.Load<Texture2D>("Asteroid");
            }
            catch (ContentLoadException)
            {
                _texture = content.Load<Texture2D>("zombie");
            }

            if (_texture != null)
            {
                float radiusMultiplier = 0.5f;
                float baseRadius = Math.Max(_texture.Width, _texture.Height) / 2f;
                float radius = baseRadius * radiusMultiplier;

                if (radius < 1f)
                {
                    radius = 1f;
                }

                _circleCollider = new CircleCollider(_initialPosition, radius);

                SetCollider(_circleCollider);
                System.Diagnostics.Debug.WriteLine($"Asteroid geladen. Collider-straal: {radius} op {_initialPosition}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Fout: Kon textuur voor Asteroid op {_initialPosition} niet laden. Collider is niet ingesteld.");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Player)
            {
                GameManager.GetGameManager().SetGameState(GameState.GameOver);
            }
            else if (other is Zombie)
            {
                GameManager.GetGameManager().RemoveGameObject(other);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null || collider is not CircleCollider circle)
            {
                base.Draw(gameTime, spriteBatch);
                return;
            }

            Vector2 origin = _texture.Bounds.Size.ToVector2() * 0.5f;

            spriteBatch.Draw(_texture, circle.Center, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);

            base.Draw(gameTime, spriteBatch);
        }
    }
}