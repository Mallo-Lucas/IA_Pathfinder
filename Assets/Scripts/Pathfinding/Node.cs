using UnityEngine;

namespace Pathfinding
{
    public class Node
    {
        public readonly bool Walkable;
        public Vector3 WorldPosition;
        public readonly int GridX;
        public readonly int GridY;
        public Node Parent;
        public float Distance;

        public Node(bool p_walkable, Vector3 p_worldPos, int p_gridX, int p_gridY)
        {
            Walkable = p_walkable;
            WorldPosition = p_worldPos;
            GridX = p_gridX;
            GridY = p_gridY;
        }
    }
}