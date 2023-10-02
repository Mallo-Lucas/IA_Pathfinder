using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private LayerMask unWalkableMask;
        [SerializeField] private Vector2 gridWorldSize;
        [SerializeField] private float nodeRadius;
        
        private Node[,] m_grid;
        private List<Node> m_nodes = new();

        private float m_nodeDiameter;
        private int m_gridSizeX, m_gridSizeY;
        
        private void Awake()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            m_nodeDiameter = nodeRadius * 2;
            m_gridSizeX = Mathf.RoundToInt(gridWorldSize.x / m_nodeDiameter);
            m_gridSizeY = Mathf.RoundToInt(gridWorldSize.y / m_nodeDiameter);
            CreateGrid();
        }
        
        private void CreateGrid()
        {
            m_grid = new Node[m_gridSizeX, m_gridSizeY];
            var l_worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            var l_radius = m_nodeDiameter - .1f;
            for (var l_x = 0; l_x < m_gridSizeX; l_x++)
            {
                for (var l_y = 0; l_y < m_gridSizeY; l_y++)
                {
                    var l_worldPoint = l_worldBottomLeft + Vector3.right * (l_x * m_nodeDiameter + nodeRadius) + Vector3.forward * (l_y * m_nodeDiameter + nodeRadius);
                    var l_walkable = !(Physics.CheckSphere(l_worldPoint, nodeRadius, unWalkableMask));
                    m_grid[l_x, l_y] = new Node(l_walkable, l_worldPoint, l_x, l_y);
                    m_nodes.Add(m_grid[l_x, l_y]);
                }
            }
        }

        public IEnumerable<Node> GetNeighbours(Node p_node)
        {
            var l_neighbours = new List<Node>();

            if (p_node.GridX-1 >-1)
                l_neighbours.Add(m_grid[(p_node.GridX - 1), p_node.GridY]);

            if (p_node.GridY - 1 > -1)
                l_neighbours.Add(m_grid[p_node.GridX, (p_node.GridY-1)]);

            if (p_node.GridX + 1 <= m_gridSizeX-1)
                l_neighbours.Add(m_grid[(p_node.GridX + 1), p_node.GridY]);

            if (p_node.GridY + 1 <= m_gridSizeY-1)
                l_neighbours.Add(m_grid[p_node.GridX, (p_node.GridY + 1)]);

            int watchDog = 0;
            
            while (!l_neighbours.Where(x=> x.Walkable).Any())
            {
                p_node = l_neighbours.LastOrDefault();
                
                if (p_node.GridX-1 >-1)
                    l_neighbours.Add(m_grid[(p_node.GridX - 1), p_node.GridY]);

                if (p_node.GridY - 1 > -1)
                    l_neighbours.Add(m_grid[p_node.GridX, (p_node.GridY-1)]);

                if (p_node.GridX + 1 <= m_gridSizeX-1)
                    l_neighbours.Add(m_grid[(p_node.GridX + 1), p_node.GridY]);

                if (p_node.GridY + 1 <= m_gridSizeY-1)
                    l_neighbours.Add(m_grid[p_node.GridX, (p_node.GridY + 1)]);
                
                watchDog++;
                if (watchDog>=10)
                    break;
            }
            
            return l_neighbours.Where(p_x=> p_x.Walkable);
        }

        public Node NodeFromWorldPoint(Vector3 p_worldPosition)
        {
            var l_position = transform.position;
            var l_percentX = ((p_worldPosition.x - l_position.x) + gridWorldSize.x / 2) / gridWorldSize.x;
            var l_percentY = ((p_worldPosition.z - l_position.z) + gridWorldSize.y / 2) / gridWorldSize.y;
            l_percentX = Mathf.Clamp01(l_percentX);
            l_percentY = Mathf.Clamp01(l_percentY);

            var l_x = Mathf.RoundToInt((m_gridSizeX - 1) * l_percentX);
            var l_y = Mathf.RoundToInt((m_gridSizeY - 1) * l_percentY);
            
            if (!m_grid[l_x, l_y].Walkable)
                return GetNeighbours(m_grid[l_x, l_y]).FirstOrDefault();

            return m_grid[l_x, l_y];
        }

        public List<Node> GetNodes() => m_nodes;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (m_grid == default) 
                return;
        
            foreach (var l_n in m_grid)
            {
                Gizmos.color = (l_n.Walkable) ? Color.white : Color.red;
                
                Gizmos.DrawCube(l_n.WorldPosition, Vector3.one * (m_nodeDiameter - .1f));
            }
        }
#endif
    }
}
