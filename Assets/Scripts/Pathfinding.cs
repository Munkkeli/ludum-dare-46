using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    public Map map;
    
    private PathRequestManager _pathRequestManager;

    private void Awake() {
        _pathRequestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector2 start, Vector2 end) {
        StartCoroutine(FindPath(start, end));
    }

    IEnumerator FindPath(Vector2 start, Vector2 target) {
        Vector2[] waypoints = new Vector2[0];
        bool isSuccess = false;

        Tile startTile = map.GetTile(start);
        Tile endTile = map.GetTile(target);

        if (startTile.isWalkable && endTile.isWalkable) {
            Heap<Tile> openSet = new Heap<Tile>(map.MaxSize);
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(startTile);

            while (openSet.Count > 0) {
                Tile currentTile = openSet.RemoveFirst();
                closedSet.Add(currentTile);

                if (currentTile == endTile) {
                    isSuccess = true;
                    break;
                }

                foreach (Tile neighbour in map.GetNeighbours(currentTile)) {
                    if (!neighbour.isWalkable || closedSet.Contains(neighbour)) continue;

                    int costToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbour) + neighbour.weight + (neighbour.isUnderHarvester ? 20 : 0);
                    if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = costToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, endTile);
                        neighbour.parent = currentTile;

                        if (!openSet.Contains(neighbour)) {
                            openSet.Add(neighbour);
                        } else {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;

        if (isSuccess) {
            waypoints = RetracePath(startTile, endTile);
        }

        _pathRequestManager.FinishedProcessingPath(waypoints, isSuccess);
    }

    Vector2[] RetracePath(Tile startTile, Tile endTile) {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;
        while (currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        // Debug.Log($"Found path {path.Count}");

        List<Vector2> waypoints = SimplifyPath(path);
        waypoints.Reverse();

        return waypoints.ToArray();
    }

    int GetDistance(Tile a, Tile b) {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        if (dx > dy) return 14 * dy + 10 * (dx - dy);
        return 14 * dx + 10 * (dy - dx);
    }

    List<Vector2> SimplifyPath(List<Tile> path) {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
            if (directionNew != directionOld) waypoints.Add(path[i].position);
            directionOld = directionNew;
        }

        return waypoints;
    }
}
