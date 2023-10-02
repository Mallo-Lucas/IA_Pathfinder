using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Pathfinding
{
    public class Pathfinder
    {
        private readonly List<Vector3> m_path = new List<Vector3>();

        public List<Vector3> GetPath() => m_path;

        private int m_indexPath;

        public void FindPath(Vector3 p_initialPos, Vector3 p_targetPos, Grid p_grid)
        {
            m_indexPath = 0;

            var l_targetNode = p_grid.NodeFromWorldPoint(p_targetPos);
            var l_seekerNode = p_grid.NodeFromWorldPoint(p_initialPos);

            var l_openSet = new List<Node>();
            var l_closedSet = new HashSet<Node>();

            var l_watchDog = 200;

            l_openSet.Add(l_seekerNode);

            while (l_watchDog > 0 || l_openSet.Count > 0)
            {
                var l_nodeZero = l_openSet[0];
                if (l_nodeZero == l_targetNode)
                    break;

                var l_neighbours = p_grid.GetNeighbours(l_nodeZero)
                    .Where(p_x => !l_closedSet.Contains(p_x) && !l_openSet.Contains(p_x));

                foreach (var l_node in l_neighbours)
                {
                    l_node.Parent = l_nodeZero;
                    l_openSet.Add(l_node);
                }

                l_closedSet.Add(l_nodeZero);
                l_openSet.Remove(l_nodeZero);
                l_openSet = OrderNodesByDistance(l_openSet, l_targetNode);
                l_watchDog--;
            }

            RetracePath(l_seekerNode, l_targetNode);
        }

        private void RetracePath(Node p_startNode, Node p_endNode)
        {
            m_path.Clear();
            var l_currentNode = p_endNode;

            while (l_currentNode != p_startNode)
            {
                m_path.Add(l_currentNode.WorldPosition);
                l_currentNode = l_currentNode.Parent;
            }

            m_path.Reverse();
        }

        private List<Node> OrderNodesByDistance(List<Node> p_nodes, Node p_targetNode)
        {
            foreach (var l_node in p_nodes)
            {
                var l_valueX = MathF.Abs(l_node.GridX - p_targetNode.GridX);
                var l_valueY = MathF.Abs(l_node.GridY - p_targetNode.GridY);
                l_node.Distance = l_valueX + l_valueY;
            }

            return p_nodes.OrderBy(p_x => p_x.Distance).ToList();
        }
        
        public Vector3 GetPoint() => m_path[m_indexPath];

        public void NextPoint()
        {
            m_indexPath++;
            if (m_indexPath >= m_path.Count)
            {
                m_indexPath = m_path.Count - 1;
            }
        }
    }
}
