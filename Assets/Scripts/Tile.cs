using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : IHeapItem<Tile>
{
    public int x;
    public int y;
    public bool isWalkable;
    public Vector2 position;
    public int weight;
    public bool isUnderHarvester = false;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    public Tile parent;

    int heapIndex;

    public Tile(int x, int y, bool isWalkable, Vector2 position, int weight) {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.position = position;
        this.weight = weight;
    }

    public int HeapIndex {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(Tile target) {
        int compare = fCost.CompareTo(target.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(target.hCost);
        }
        return -compare;
    }
}
