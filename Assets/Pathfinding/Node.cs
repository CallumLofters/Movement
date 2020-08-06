using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridZ;
    public float gCost;
    public float hCost;
    public Node parent;
    int heapIndex;
    public int visited = 0;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridZ)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridZ = _gridZ;
    }

    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
