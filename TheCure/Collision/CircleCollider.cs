using System;
using TheCure.Collision;
using Microsoft.Xna.Framework;

namespace TheCure
{
    public class CircleCollider : Collider, IEquatable<CircleCollider>
    {
        public float X;
        public float Y;
        public Vector2 Center
        {
            get
            {
                return new Vector2(X, Y);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        public float Radius;

        public CircleCollider(float x, float y, float radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }

        public CircleCollider(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public override bool Contains(Vector2 coordinates)
        {
            return (Center - coordinates).Length() < Radius;
        }

        public override bool Intersects(CircleCollider tmp)
        {
            float distanceBetweenCenters = Vector2.Distance(Center, tmp.Center);
            float sumOfRadius = Radius + tmp.Radius;

            return distanceBetweenCenters < sumOfRadius;
        }

        public override bool Intersects(RectangleCollider tmp)
        {
            if (IsPointInsideCircle(tmp.shape.Location.ToVector2()))
                return true;

            if (IsPointInsideCircle(new Vector2(tmp.shape.Right, tmp.shape.Top)))
                return true;

            if (IsPointInsideCircle(new Vector2(tmp.shape.Left, tmp.shape.Bottom)))
                return true;

            if (IsPointInsideCircle(tmp.shape.Location.ToVector2() + tmp.shape.Size.ToVector2()))
                return true;

            if (IsLineIntersectingCircle(tmp.shape.Top, tmp.shape.Left, tmp.shape.Top, tmp.shape.Right))
                return true;

            if (IsLineIntersectingCircle(tmp.shape.Bottom, tmp.shape.Left, tmp.shape.Bottom, tmp.shape.Right))
                return true;

            if (IsLineIntersectingCircle(tmp.shape.Left, tmp.shape.Top, tmp.shape.Left, tmp.shape.Bottom))
                return true;

            if (IsLineIntersectingCircle(tmp.shape.Right, tmp.shape.Top, tmp.shape.Right, tmp.shape.Bottom))
                return true;

            if (tmp.shape.Contains(Center))
                return true;

            return false;
        }

        public override bool Intersects(LinePieceCollider tmp)
        {
            return tmp.Intersects(this);
        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)(X - Radius), (int)(Y - Radius), (int)(2 * Radius), (int)(2 * Radius));
        }

        public bool Equals(CircleCollider tmp)
        {
            return tmp.X == X && tmp.Y == Y && tmp.Radius == Radius;
        }

        private bool IsPointInsideCircle(Vector2 point)
        {
            return Vector2.Distance(point, Center) < Radius;
        }

        private bool IsLineIntersectingCircle(float x1, float y1, float x2, float y2)
        {
            Vector2 startPosition = new Vector2(x1, y1);
            Vector2 endPosition = new Vector2(x2, y2);

            Vector2 lineDirection = endPosition - startPosition;
            Vector2 circleToStart = startPosition - Center;

            float a = Vector2.Dot(lineDirection, lineDirection);
            float b = 2 * Vector2.Dot(circleToStart, lineDirection);
            float c = Vector2.Dot(circleToStart, circleToStart) - Radius * Radius;

            float sum = b * b - 4 * a * c;

            if (sum < 0)
            {
                return false;
            }
            else if (sum == 0)
            {
                float t = -b / (2 * a);
                return t >= 0 && t <= 1;
            }
            else
            {
                float t1 = (-b - MathF.Sqrt(sum)) / (2 * a);
                float t2 = (-b + MathF.Sqrt(sum)) / (2 * a);

                return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
            }
        }
    }
}