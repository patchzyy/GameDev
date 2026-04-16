using System;
using TheCure.Collision;
using Microsoft.Xna.Framework;

namespace TheCure
{
    public class LinePieceCollider : Collider, IEquatable<LinePieceCollider>
    {
        public Vector2 Start;
        public Vector2 End;

        public float Length
        {
            get
            {
                return (End - Start).Length();
            }
            set
            {
                End = Start + GetDirection() * value;
            }
        }

        public float StandardA
        {
            get
            {
                return End.Y - Start.Y;
            }
        }

        public float StandardB
        {
            get
            {
                return Start.X - End.X;
            }
        }

        public float StandardC
        {
            get
            {
                return (End.X - Start.X) * Start.Y - (End.Y - Start.Y) * Start.X;
            }
        }

        public LinePieceCollider(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public LinePieceCollider(Vector2 start, Vector2 direction, float length)
        {
            Start = start;

            Vector2 normalizedDirection = direction;
                    normalizedDirection.Normalize();

            End = start + normalizedDirection * length;
        }

        public static float GetAngle(Vector2 direction)
        {
            direction.Normalize();

            return (float)Math.Atan2(direction.X, -direction.Y);
        }

        public static Vector2 GetDirection(Vector2 point1, Vector2 point2)
        {
            Vector2 direction = point2 - point1;
            direction.Normalize();

            return direction;
        }

        public override bool Intersects(LinePieceCollider tmp)
        {
            return LineSegmentsIntersect(Start, End, tmp.Start, tmp.End);
        }

        public override bool Intersects(CircleCollider tmp)
        {
            Vector2 closestPoint = NearestPointOnLine(tmp.Center);

            float distance = Vector2.Distance(tmp.Center, closestPoint);

            return distance <= tmp.Radius;
        }

        public override bool Intersects(RectangleCollider tmp)
        {
            Vector2 rectTopLeft = tmp.shape.Location.ToVector2();
            Vector2 rectTopRight = new Vector2(tmp.shape.Right, tmp.shape.Top);
            Vector2 rectBottomLeft = new Vector2(tmp.shape.Left, tmp.shape.Bottom);
            Vector2 rectBottomRight = tmp.shape.Location.ToVector2() + tmp.shape.Size.ToVector2();

            bool intersects = LineSegmentsIntersect(Start, End, rectTopLeft, rectTopRight) ||
                              LineSegmentsIntersect(Start, End, rectTopRight, rectBottomRight) ||
                              LineSegmentsIntersect(Start, End, rectBottomRight, rectBottomLeft) ||
                              LineSegmentsIntersect(Start, End, rectBottomLeft, rectTopLeft);

            if (!intersects && (tmp.Contains(Start) || IsPointOnRectangleEdge(Start, tmp.shape)) && (tmp.Contains(End) || IsPointOnRectangleEdge(End, tmp.shape)))
            {
                intersects = true;
            }

            return intersects;
        }

        private bool IsPointOnRectangleEdge(Vector2 point, Rectangle rectangle)
        {
            return (point.X == rectangle.Left || point.X == rectangle.Right) && (point.Y >= rectangle.Top && point.Y <= rectangle.Bottom) ||
                   (point.Y == rectangle.Top || point.Y == rectangle.Bottom) && (point.X >= rectangle.Left && point.X <= rectangle.Right);
        }

        private bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float determine = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);

            if (determine == 0)
                return false;

            float t = ((p1.X - p3.X) * (p3.Y - p4.Y) - (p1.Y - p3.Y) * (p3.X - p4.X)) / determine;
            float u = -((p1.X - p2.X) * (p1.Y - p3.Y) - (p1.Y - p2.Y) * (p1.X - p3.X)) / determine;

            return t > 0 && t < 1 && u > 0 && u < 1;
        }

        public Vector2 GetIntersection(LinePieceCollider tmp)
        {
            float a1 = StandardA;
            float b1 = StandardB;
            float c1 = StandardC;

            float a2 = tmp.StandardA;
            float b2 = tmp.StandardB;
            float c2 = tmp.StandardC;

            float determine = a1 * b2 - a2 * b1;

            if (determine == 0)
                return Vector2.Zero;

            float x = (b1 * c2 - b2 * c1) / determine;
            float y = (a2 * c1 - a1 * c2) / determine;

            return new Vector2(x, y);
        }

        public Vector2 NearestPointOnLine(Vector2 point)
        {
            Vector2 lineDirection = GetDirection();
            Vector2 lineStartToPoint = point - Start;

            float projectionLength = Vector2.Dot(lineStartToPoint, lineDirection);
                  projectionLength = Math.Max(0, Math.Min(Length, projectionLength));

            return Start + lineDirection * projectionLength;
        }

        public override Rectangle GetBoundingBox()
        {
            Point topLeft = new Point((int)Math.Min(Start.X, End.X), (int)Math.Min(Start.Y, End.Y));
            Point size = new Point((int)Math.Max(Start.X, End.X), (int)Math.Max(Start.Y, End.X)) - topLeft;

            return new Rectangle(topLeft, size);
        }

        public override bool Contains(Vector2 coordinates)
        {
            float tolerance = 0.001f;

            if (coordinates.X < Math.Min(Start.X, End.X) - tolerance ||
                coordinates.X > Math.Max(Start.X, End.X) + tolerance ||
                coordinates.Y < Math.Min(Start.Y, End.Y) - tolerance ||
                coordinates.Y > Math.Max(Start.Y, End.Y) + tolerance)
                return false;

            float crossProduct = (coordinates.Y - Start.Y) * (End.X - Start.X) - (coordinates.X - Start.X) * (End.Y - Start.Y);

            return Math.Abs(crossProduct) < tolerance;
        }

        public bool Equals(LinePieceCollider tmp)
        {
            return tmp.Start == Start && tmp.End == End;
        }

        public static Vector2 GetDirection(Point point1, Point point2)
        {
            return GetDirection(point1.ToVector2(), point2.ToVector2());
        }

        public Vector2 GetDirection()
        {
            return GetDirection(Start, End);
        }

        public float GetAngle()
        {
            return GetAngle(GetDirection());
        }
    }
}