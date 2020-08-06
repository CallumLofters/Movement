using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool onlyDisplayPathGizmos;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeZ;


    void Start()
    {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
            CreateGrid();
    }

    public void ResetGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeZ;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2; // NEED TO CHANGE HERE

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, z] = new Node(walkable, worldPoint, x, z);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0 || (x != 0 && z != 0))  // IF X == Y CONTINUE FOR ONLY UP/DOWN/LEFT/RIGHT NO DIAG
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;

                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neighbours.Add(grid[checkX, checkZ]);
                }
            }
        }
        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        return grid[x, z];
    }



    public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.z));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
                }
            }
        }
        else
        {
            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null)
                    {
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
                    }

                }
            }
        }

    }
    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.z));

    //        if (grid != null)
    //        {
    //            foreach (Node n in grid)
    //            {
    //                Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));               
    //            }
    //        }

    //}

}
