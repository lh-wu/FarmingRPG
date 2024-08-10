using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost;
    public int hCost;
    public bool isObstacle = false;
    public int movementPenalty;
    public Node parentNode;

    public int FCost { get { return gCost + hCost; } }


    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }


    public int CompareTo(Node other)
    {
        int compare = this.FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = this.hCost.CompareTo(other.hCost);
        }
        return compare;
    }
}
