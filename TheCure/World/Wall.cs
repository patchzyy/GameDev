using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Collision;
using TheCure.Managers;

namespace TheCure
{
    public class Wall : GameObject
    {
        private readonly RectangleCollider _rectangleCollider;
        private Texture2D _texture;

        public Wall(Rectangle bounds)
        {
            _rectangleCollider = new(bounds);
            SetCollider(_rectangleCollider);
        }

        public override void Load()
        {
            var content = ContentsManager.Get().GetContent();
            _texture = content.Load<Texture2D>("Wall_Texture");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null)
                return;

            var area = _rectangleCollider.shape;

            for (var x = area.Left; x < area.Right; x += _texture.Width)
            {
                for (var y = area.Top; y < area.Bottom; y += _texture.Height)
                {
                    var tileWidth = Math.Min(_texture.Width, area.Right - x);
                    var tileHeight = Math.Min(_texture.Height, area.Bottom - y);

                    spriteBatch.Draw(
                        _texture,
                        new Rectangle(x, y, tileWidth, tileHeight),
                        new Rectangle(0, 0, tileWidth, tileHeight),
                        Color.White);
                }
            }
        }

        public void ResolveRectangleCollision(RectangleCollider mover, Rectangle previousBounds, ref Vector2 velocity)
        {
            var wall = _rectangleCollider.shape;
            var current = mover.shape;

            if (previousBounds.Bottom <= wall.Top)
            {
                current.Y = wall.Top - current.Height;
                velocity.Y = 0f;
            }
            else if (previousBounds.Top >= wall.Bottom)
            {
                current.Y = wall.Bottom;
                velocity.Y = 0f;
            }
            else if (previousBounds.Right <= wall.Left)
            {
                current.X = wall.Left - current.Width;
                velocity.X = 0f;
            }
            else if (previousBounds.Left >= wall.Right)
            {
                current.X = wall.Right;
                velocity.X = 0f;
            }
            else
            {
                var overlapLeft = current.Right - wall.Left;
                var overlapRight = wall.Right - current.Left;
                var overlapTop = current.Bottom - wall.Top;
                var overlapBottom = wall.Bottom - current.Top;
                var smallestOverlap =
                    Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom));

                if (smallestOverlap == overlapLeft)
                {
                    current.X = wall.Left - current.Width;
                    velocity.X = 0f;
                }
                else if (smallestOverlap == overlapRight)
                {
                    current.X = wall.Right;
                    velocity.X = 0f;
                }
                else if (smallestOverlap == overlapTop)
                {
                    current.Y = wall.Top - current.Height;
                    velocity.Y = 0f;
                }
                else
                {
                    current.Y = wall.Bottom;
                    velocity.Y = 0f;
                }
            }

            mover.shape = current;
        }

        //todo: kijk hier extra naar voor de Friendly collision bug die we hebben, Ik weet niet of het hieraan ligt of aan de friendly logica zelf.
        public Vector2 ResolveCircleCollision(CircleCollider mover, Vector2 previousCenter)
        {
            var wall = _rectangleCollider.shape;
            var currentCenter = mover.Center;
            Vector2 collisionNormal;

            if (previousCenter.Y + mover.Radius <= wall.Top)
            {
                currentCenter.Y = wall.Top - mover.Radius;
                collisionNormal = -Vector2.UnitY;
            }
            else if (previousCenter.Y - mover.Radius >= wall.Bottom)
            {
                currentCenter.Y = wall.Bottom + mover.Radius;
                collisionNormal = Vector2.UnitY;
            }
            else if (previousCenter.X + mover.Radius <= wall.Left)
            {
                currentCenter.X = wall.Left - mover.Radius;
                collisionNormal = -Vector2.UnitX;
            }
            else if (previousCenter.X - mover.Radius >= wall.Right)
            {
                currentCenter.X = wall.Right + mover.Radius;
                collisionNormal = Vector2.UnitX;
            }
            else
            {
                var overlapLeft = currentCenter.X + mover.Radius - wall.Left;
                var overlapRight = wall.Right - (currentCenter.X - mover.Radius);
                var overlapTop = currentCenter.Y + mover.Radius - wall.Top;
                var overlapBottom = wall.Bottom - (currentCenter.Y - mover.Radius);
                if (overlapLeft <= overlapRight && overlapLeft <= overlapTop && overlapLeft <= overlapBottom)
                {
                    currentCenter.X = wall.Left - mover.Radius;
                    collisionNormal = -Vector2.UnitX;
                }
                else if (overlapRight <= overlapTop && overlapRight <= overlapBottom)
                {
                    currentCenter.X = wall.Right + mover.Radius;
                    collisionNormal = Vector2.UnitX;
                }
                else if (overlapTop <= overlapBottom)
                {
                    currentCenter.Y = wall.Top - mover.Radius;
                    collisionNormal = -Vector2.UnitY;
                }
                else
                {
                    currentCenter.Y = wall.Bottom + mover.Radius;
                    collisionNormal = Vector2.UnitY;
                }
            }

            mover.Center = currentCenter;
            return collisionNormal;
        }
    }
}