using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float spawnInterval = 2;
    public Transform target;
    public GameObject enemy;

    private float spawnTimer = 0;
    private Vector2[] path;

    Vector2[] FindPath(Vector2 start, Vector2 target) {
        Tile startTile = Map.current.GetTile(start);
        Tile endTile = Map.current.GetTile(target);

        List<Tile> openSet = new List<Tile>();
        List<Tile> closedSet = new List<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0) {
            Tile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentTile.fCost || (openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost)) {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if (currentTile == endTile) {
                return RetracePath(startTile, endTile);
            }

            foreach (Tile neighbour in Map.current.GetNeighbours(currentTile)) {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour)) continue;

                int costToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbour);
                if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endTile);
                    neighbour.parent = currentTile;
                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return new Vector2[] { };
    }

    Vector2[] RetracePath(Tile startTile, Tile endTile) {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;
        while (currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        Debug.Log($"Found path {path.Count}");

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

    // Start is called before the first frame update
    void Start() {
        spawnTimer = spawnInterval;

        if (target == null) {
            target = GameObject.Find("Turret").transform;
        }

        path = FindPath(transform.position, target.position);
    }

    // Update is called once per frame
    void Update() {
        if (spawnTimer > 0) {
            spawnTimer -= Time.deltaTime;
        } else {
            spawnTimer = spawnInterval + spawnTimer;
            GameObject instance = Instantiate(enemy, transform.position, Quaternion.identity);
            instance.GetComponent<Enemy>().path = path;
        }
    }
}
