using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.World;

//bronnen: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW

namespace TheCure
{
    //Node
    public class PathNode
    {
        public int GridX, GridY;
        public bool IsWalkable;
        
        // Voor A*
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;
        public PathNode Parent;

        public PathNode(int gridX, int gridY, bool isWalkable)
        {
            GridX = gridX;
            GridY = gridY;
            IsWalkable = isWalkable;
        }
    }

    public static class Pathfinder
    {
        private static PathNode[,] _grid;
        private static int _gridWidth;
        private static int _gridHeight;
        
        // groot = simpeler maar sneller aangezien game niet heel erg precies hoeft te zijn is dit goed
        public const int TileSize = 64; 
        private const int StartX = -1800;
        private const int StartY = -1200;

        public static void InitGrid(List<Rectangle> obstacleBounds)
        {
            _gridWidth = 3600 / TileSize;
            _gridHeight = 2400 / TileSize;
            _grid = new PathNode[_gridWidth, _gridHeight];

            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    Rectangle tileRect = new Rectangle(StartX + x * TileSize, StartY + y * TileSize, TileSize, TileSize);
                    bool isWalkable = true;

                    // Super simpele collision check per tegel
                    foreach (var bound in obstacleBounds)
                    {
                        if (bound.Intersects(tileRect))
                        {
                            isWalkable = false;
                            break;
                        }
                    }

                    _grid[x, y] = new PathNode(x, y, isWalkable);
                }
            }
        }

        public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
        {
            if (_grid == null) return null;

            int startX = (int)((startPos.X - StartX) / TileSize);
            int startY = (int)((startPos.Y - StartY) / TileSize);
            int targetX = (int)((targetPos.X - StartX) / TileSize);
            int targetY = (int)((targetPos.Y - StartY) / TileSize);

            // Simpele bounds check
            if (startX < 0 || startX >= _gridWidth || startY < 0 || startY >= _gridHeight ||
                targetX < 0 || targetX >= _gridWidth || targetY < 0 || targetY >= _gridHeight)
            {
                return null;
            }

            PathNode startNode = _grid[startX, startY];
            PathNode targetNode = _grid[targetX, targetY];

            if (!targetNode.IsWalkable)
                return null;

            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();

            //  simpel reset voor we beginnen 
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    _grid[x, y].GCost = int.MaxValue;
                    _grid[x, y].Parent = null;
                }
            }

            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, targetNode);
            openList.Add(startNode);

            // Max stappen om niet vast te lopen
            int loopCounter = 0; 
            while (openList.Count > 0 && loopCounter < 300)
            {
                loopCounter++;

                PathNode currentNode = openList[0];

                // Vind node met laagste kosten
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].FCost < currentNode.FCost || 
                       (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);
                

                foreach (PathNode neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                        continue;
                    

                    int newMovementCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (newMovementCost < neighbor.GCost || !openList.Contains(neighbor))
                    {

                        neighbor.GCost = newMovementCost;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private static List<PathNode> GetNeighbors(PathNode node)
        {
            List<PathNode> neighbors = new List<PathNode>();

            // simpel 4
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            for (int i = 0; i < 4; i++)
            {
                int checkX = node.GridX + dx[i];
                int checkY = node.GridY + dy[i];

                if (checkX >= 0 && checkX < _gridWidth && checkY >= 0 && checkY < _gridHeight)
                {
                    neighbors.Add(_grid[checkX, checkY]);
                }
            }

            return neighbors;
        }

        private static int GetDistance(PathNode nodeA, PathNode nodeB)
        {
            int dstX = Math.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Math.Abs(nodeA.GridY - nodeB.GridY);
            // Manhattan distance zoals in de les
            return dstX + dstY;
        }

        private static List<Vector2> RetracePath(PathNode startNode, PathNode endNode)
        {
            List<Vector2> path = new List<Vector2>();
            PathNode currentNode = endNode;

            while (currentNode != startNode && currentNode != null)
            {
                path.Add(new Vector2(StartX + currentNode.GridX * TileSize + TileSize / 2f, 
                                     StartY + currentNode.GridY * TileSize + TileSize / 2f));
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}
