using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    internal class Laser : GameObject
    {
        private LinePieceCollider line;
        private Texture2D sprite;
        private double lifespan = .20f;

        public Laser(LinePieceCollider piece)
        {
            this.line = piece;
            SetCollider(piece);
        }
        public Laser(LinePieceCollider piece, float length) : this(piece)
        {
            this.line.Length = length;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            sprite = content.Load<Texture2D>("Laser");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (lifespan < 0)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }

            lifespan -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle target = new Rectangle(
                (int)line.Start.X,
                (int)line.Start.Y,
                (int)sprite.Width,
                (int)line.Length
            );

            spriteBatch.Draw(sprite, target, null, Color.White, line.GetAngle(), new Vector2(sprite.Width / 2f, sprite.Height), SpriteEffects.None, 1);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
