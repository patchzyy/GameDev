using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure
{
    internal static class ProceduralWorldGenerator
    {
        private const string WorldObjectsTexture = "Objects";
        private const string GrassPatchesTexture = "BG-GrassPatches";
        private const float Scale = 2f;
        private const int PlacementPadding = 48;
        private const int PlayerSafeRadius = 240;

        private static readonly Rectangle[] GrassPatchSources =
        {
            new(0, 0, 160, 96),
            new(160, 0, 160, 96),
            new(320, 0, 160, 96)
        };

        private static readonly ObstacleDefinition[] BuildingDefinitions =
        {
            new("TallWatchTower", new Rectangle(215, 93, 41, 46), new Rectangle(5, 28, 31, 15)),
            new("SmallWatchTower", new Rectangle(170, 95, 33, 44), new Rectangle(4, 28, 25, 14)),
            new("SmallerWatchTower", new Rectangle(126, 101, 27, 38), new Rectangle(4, 24, 20, 12)),
            new("TinyWatchTower", new Rectangle(53, 112, 27, 27), new Rectangle(4, 17, 19, 8)),
            new("WoodenPlankGround", new Rectangle(20, 116, 23, 18), new Rectangle(1, 10, 21, 8))
        };

        private static readonly ObstacleDefinition HorizontalWallDefinition = new("HorizontalWall", new Rectangle(160, 6, 55, 16), new Rectangle(0, 2, 55, 12));

        private static readonly ObstacleDefinition VerticalWallDefinition = new("VerticalWall", new Rectangle(139, 24, 13, 46), new Rectangle(1, 0, 11, 46));

        private static readonly ObstacleDefinition[] GroundSpikeDefinitions =
        {
            new("WoodSpikeInGroundFacingLeft", new Rectangle(256, 25, 18, 23), new Rectangle(3, 4, 12, 15)),
            new("WoodSpikeInGroundFacingRight", new Rectangle(259, 64, 16, 21), new Rectangle(2, 4, 11, 14))
        };

        public static void Generate(GameManager gameManager, Action<GameObject> addStaticObject)
        {
            var occupied = new List<Rectangle>();
            var playerPosition = PlayerManager.Get().Player.GetPosition();
            occupied.Add(CreateCenteredRectangle(playerPosition, PlayerSafeRadius * 2, PlayerSafeRadius * 2));

            AddGrassPatches(gameManager, addStaticObject);
            AddBuildings(gameManager, addStaticObject, occupied);
            AddWoodenWallGroups(gameManager, addStaticObject, occupied);
            AddGroundSpikeGroups(gameManager, addStaticObject, occupied);
        }

        private static void AddGrassPatches(GameManager gameManager, Action<GameObject> addStaticObject)
        {
            var playableBounds = gameManager.GetPlayableBounds();

            for (var i = 0; i < 36; i++)
            {
                var source = GrassPatchSources[gameManager.RNG.Next(GrassPatchSources.Length)];
                var destination = CreateRandomDestination(gameManager, playableBounds, source, 1.5f);

                addStaticObject(new DecorativeSprite(
                    $"GrassPatch_{i}",
                    GrassPatchesTexture,
                    source,
                    destination));
            }
        }

        private static void AddBuildings(
            GameManager gameManager,
            Action<GameObject> addStaticObject,
            List<Rectangle> occupied)
        {
            var playableBounds = gameManager.GetPlayableBounds();

            for (var i = 0; i < 14; i++)
            {
                var definition = BuildingDefinitions[gameManager.RNG.Next(BuildingDefinitions.Length)];
                if (!TryFindPlacement(gameManager, playableBounds, definition.SourceRectangle, Scale, occupied, 40, out Rectangle destinationRectangle))
                    continue;

                var collisionRectangle = ScaleAndOffsetRectangle(
                    definition.CollisionRectangle,
                    destinationRectangle,
                    definition.SourceRectangle);

                occupied.Add(InflateRectangle(collisionRectangle, PlacementPadding));

                addStaticObject(new SceneryObstacle(
                    definition.Name,
                    destinationRectangle,
                    collisionRectangle,
                    definition.SourceRectangle));
            }
        }

        private static void AddWoodenWallGroups(
            GameManager gameManager,
            Action<GameObject> addStaticObject,
            List<Rectangle> occupied)
        {
            var playableBounds = gameManager.GetPlayableBounds();

            for (var groupIndex = 0; groupIndex < 8; groupIndex++)
            {
                bool horizontal = gameManager.RNG.NextDouble() < 0.6;
                int segmentCount = gameManager.RNG.Next(3, 5);
                var definition = horizontal ? HorizontalWallDefinition : VerticalWallDefinition;
                Point segmentSize = new((int)(definition.SourceRectangle.Width * Scale), (int)(definition.SourceRectangle.Height * Scale));

                Rectangle groupFootprint = horizontal
                    ? new Rectangle(0, 0, segmentSize.X * segmentCount, segmentSize.Y)
                    : new Rectangle(0, 0, segmentSize.X, segmentSize.Y * segmentCount);

                if (!TryFindPlacement(gameManager, playableBounds, groupFootprint.Size, occupied, 48, out Point origin))
                    continue;

                var groupCollision = Rectangle.Empty;
                for (var segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
                {
                    Rectangle destinationRectangle = horizontal
                        ? new Rectangle(origin.X + segmentIndex * segmentSize.X, origin.Y,
                            segmentSize.X, segmentSize.Y)
                        : new Rectangle(origin.X, origin.Y + segmentIndex * segmentSize.Y,
                            segmentSize.X, segmentSize.Y);

                    Rectangle collisionRectangle = ScaleAndOffsetRectangle(
                        definition.CollisionRectangle,
                        destinationRectangle,
                        definition.SourceRectangle);

                    groupCollision = groupCollision == Rectangle.Empty
                        ? collisionRectangle
                        : Rectangle.Union(groupCollision, collisionRectangle);

                    addStaticObject(new SceneryObstacle(
                        definition.Name,
                        destinationRectangle,
                        collisionRectangle,
                        definition.SourceRectangle));
                }

                occupied.Add(InflateRectangle(groupCollision, PlacementPadding));
            }
        }

        private static void AddGroundSpikeGroups(
            GameManager gameManager,
            Action<GameObject> addStaticObject,
            List<Rectangle> occupied)
        {
            var playableBounds = gameManager.GetPlayableBounds();

            for (var groupIndex = 0; groupIndex < 7; groupIndex++)
            {
                var definition = GroundSpikeDefinitions[gameManager.RNG.Next(GroundSpikeDefinitions.Length)];
                int segmentCount = gameManager.RNG.Next(3, 5);
                Point segmentSize = new(
                    (int)(definition.SourceRectangle.Width * Scale),
                    (int)(definition.SourceRectangle.Height * Scale));
                Rectangle groupFootprint = new(0, 0, segmentSize.X * segmentCount, segmentSize.Y);

                if (!TryFindPlacement(gameManager, playableBounds, groupFootprint.Size, occupied, 40, out Point origin))
                    continue;

                var groupCollision = Rectangle.Empty;
                for (var segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
                {
                    Rectangle destinationRectangle = new(
                        origin.X + segmentIndex * (segmentSize.X - 4),
                        origin.Y + gameManager.RNG.Next(-2, 3),
                        segmentSize.X,
                        segmentSize.Y);

                    Rectangle collisionRectangle = ScaleAndOffsetRectangle(
                        definition.CollisionRectangle,
                        destinationRectangle,
                        definition.SourceRectangle);

                    groupCollision = groupCollision == Rectangle.Empty
                        ? collisionRectangle
                        : Rectangle.Union(groupCollision, collisionRectangle);

                    addStaticObject(new SceneryObstacle(
                        definition.Name,
                        destinationRectangle,
                        collisionRectangle,
                        definition.SourceRectangle));
                }

                occupied.Add(InflateRectangle(groupCollision, PlacementPadding));
            }
        }

        private static Rectangle CreateRandomDestination(
            GameManager gameManager,
            Rectangle playableBounds,
            Rectangle sourceRectangle,
            float scale)
        {
            int width = (int)(sourceRectangle.Width * scale);
            int height = (int)(sourceRectangle.Height * scale);

            return new Rectangle(
                gameManager.RNG.Next(playableBounds.Left, playableBounds.Right - width),
                gameManager.RNG.Next(playableBounds.Top, playableBounds.Bottom - height),
                width,
                height);
        }

        private static bool TryFindPlacement(
            GameManager gameManager,
            Rectangle playableBounds,
            Rectangle sourceRectangle,
            float scale,
            List<Rectangle> occupied,
            int padding,
            out Rectangle destinationRectangle)
        {
            var size = new Point((int)(sourceRectangle.Width * scale), (int)(sourceRectangle.Height * scale));
            if (TryFindPlacement(gameManager, playableBounds, size, occupied, padding, out Point location))
            {
                destinationRectangle = new Rectangle(location.X, location.Y, size.X, size.Y);
                return true;
            }

            destinationRectangle = Rectangle.Empty;

            return false;
        }

        private static bool TryFindPlacement(
            GameManager gameManager,
            Rectangle playableBounds,
            Point size,
            List<Rectangle> occupied,
            int padding,
            out Point location)
        {
            for (var attempt = 0; attempt < 80; attempt++)
            {
                var candidate = new Rectangle(
                    gameManager.RNG.Next(playableBounds.Left, playableBounds.Right - size.X),
                    gameManager.RNG.Next(playableBounds.Top, playableBounds.Bottom - size.Y),
                    size.X,
                    size.Y);

                var blockedCandidate = InflateRectangle(candidate, padding);
                bool overlaps = false;

                foreach (var rectangle in occupied)
                {
                    if (!blockedCandidate.Intersects(rectangle))
                        continue;

                    overlaps = true;
                    break;
                }

                if (overlaps)
                    continue;

                location = candidate.Location;
                return true;
            }

            location = Point.Zero;
            return false;
        }

        private static Rectangle ScaleAndOffsetRectangle(
            Rectangle localRectangle,
            Rectangle destinationRectangle,
            Rectangle sourceRectangle)
        {
            float scaleX = destinationRectangle.Width / (float)sourceRectangle.Width;
            float scaleY = destinationRectangle.Height / (float)sourceRectangle.Height;

            return new Rectangle(
                destinationRectangle.X + (int)(localRectangle.X * scaleX),
                destinationRectangle.Y + (int)(localRectangle.Y * scaleY),
                Math.Max(1, (int)(localRectangle.Width * scaleX)),
                Math.Max(1, (int)(localRectangle.Height * scaleY)));
        }

        private static Rectangle InflateRectangle(Rectangle rectangle, int amount)
        {
            var inflated = rectangle;
            inflated.Inflate(amount, amount);

            return inflated;
        }

        private static Rectangle CreateCenteredRectangle(Vector2 center, int width, int height)
        {
            return new Rectangle(
                (int)center.X - width / 2,
                (int)center.Y - height / 2,
                width,
                height);
        }

        private readonly record struct ObstacleDefinition(
            string Name,
            Rectangle SourceRectangle,
            Rectangle CollisionRectangle);
    }
}
