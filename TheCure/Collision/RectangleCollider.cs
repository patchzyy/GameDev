using System;
using Microsoft.Xna.Framework;

namespace TheCure.Collision
{
    public class RectangleCollider : Collider, IEquatable<RectangleCollider>
    {
        public Rectangle shape;

        public RectangleCollider(Rectangle shape)
        {
            this.shape = shape;
        }

        public override bool Contains(Vector2 location)
        {
            return shape.Contains(location);
        }

        public bool Equals(RectangleCollider tmp)
        {
            return shape == tmp.shape;
        }

        public override Rectangle GetBoundingBox()
        {
            return shape;
        }

        public override bool Intersects(CircleCollider tmp)
        {
            return tmp.Intersects(this);
        }

        public override bool Intersects(RectangleCollider tmp)
        {
            return shape.Intersects(tmp.shape);
        }

        public override bool Intersects(LinePieceCollider tmp)
        {
            return tmp.Intersects(this);
        }
    }
}
