using System;
using Microsoft.Xna.Framework;

namespace TheCure.Collision
{
    public abstract class Collider
    {
        public abstract Rectangle GetBoundingBox();

        public bool CheckIntersection(Collider tmp)
        {
            switch (tmp)
            {
                case RectangleCollider:
                    return Intersects((RectangleCollider)tmp);
                case CircleCollider:
                    return Intersects((CircleCollider)tmp);
                case LinePieceCollider:
                    return Intersects((LinePieceCollider)tmp);
                case null:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        public abstract bool Intersects(CircleCollider tmp);

        public abstract bool Intersects(RectangleCollider tmp);

        public abstract bool Intersects(LinePieceCollider tmp);

        public abstract bool Contains(Vector2 location);
    }
}
