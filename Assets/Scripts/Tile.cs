using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int x;
    public int y;
    public bool isWalkable;
    public Vector2 position;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    public Tile parent;

    public Tile(int x, int y, bool isWalkable, Vector2 position) {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.position = position;
    }
}
