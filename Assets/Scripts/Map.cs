using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map current;

    public float turretArea = 10;
    public int size = 64;
    public float tileSize = 1;

    public Renderer map;
    public Transform turret;
    public GameObject spruceTree;
    public GameObject spawn;

    public Tile[,] tiles;

    private Vector2[] _roadLines;
    public Vector2[] _spawnPoints;

    // Start is called before the first frame update
    void Awake()
    {
        Vector2 center = new Vector2(tileSize * (size / 2), tileSize * (size / 2));

        // Create 3 lines facing a random direction (But not too close to each other)
        List<Vector2> roadLines = new List<Vector2>();
        while (roadLines.Count < 3) {
            Vector2 direction = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))) * Vector2.up;
            if (roadLines.Count > 0) {
                bool isSuitable = true;
                foreach (Vector2 line in roadLines) {
                    if (Vector2.Distance(direction * 5f, line * 5f) < 5f) {
                        isSuitable = false;
                        break;
                    }
                }

                if (!isSuitable) continue;
            }

            roadLines.Add(direction);
        }
        _roadLines = roadLines.ToArray();

        // Find out where spawn points should be based on the road lines
        List<Vector2> spawnPoints = new List<Vector2>();
        foreach (Vector2 line in _roadLines) {
            Vector2[] corners = new Vector2[] {
                center - new Vector2(0, 0),
                center - new Vector2(size * tileSize, 0),
                center - new Vector2(size * tileSize, size * tileSize),
                center - new Vector2(0, size * tileSize),
            };

            for (int i = 1; i < 5; i++) {
                Vector2? point = LineLineIntersection(Vector2.zero, line * 10f, corners[i - 1], corners[i % 4]);
                if (point != null) {
                    spawnPoints.Add((Vector2)point);

                    // TODO: Remove this
                    Instantiate(spawn, (Vector2)point, Quaternion.identity);

                    break;
                }
            }
        }
        _spawnPoints = spawnPoints.ToArray();

        Map.current = this;

        tiles = new Tile[size, size];

        float[] tileTypeArray = new float[30 * 30];
        float downgrade = size / 30f;

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Vector2 variation = new Vector2(0.5f - Random.value, 0.5f - Random.value) * tileSize;

                Vector2 position = center - (new Vector2(x * tileSize, y * tileSize));
                position -= new Vector2(tileSize / 2, 0);

                float height = CalculateTileState(position);

                // Protect turret area
                // height = Mathf.Clamp01(height - Mathf.Max(0, turretArea - Vector2.Distance(turret.position, position)));

                position += variation;

                if (height > 0.5f) {
                    GameObject instance = Instantiate(spruceTree, position, Quaternion.identity);
                    instance.transform.localScale = new Vector2(1, 1) * ((height * 4) + Random.value);
                }

                tileTypeArray[((int)((x / downgrade)) * 30) + (int)(y / downgrade)] = height;
                tiles[x, y] = new Tile(x, y, height < 0.4f, center - (new Vector2(x * tileSize, y * tileSize)));
            }
        }

        map.sharedMaterial.SetFloatArray("_TileType", tileTypeArray);
        map.gameObject.transform.localScale = new Vector3(size * tileSize * 0.1f, 1, size * tileSize * 0.1f);
    }

    private float CalculateTileState(Vector2 position) {
        // Initial state from perlin noise
        float state = Mathf.PerlinNoise(position.x * 0.75f, position.y * 0.75f) - 0.1f;

        // More forest at the end of the map
        state += Mathf.Clamp(Vector2.Distance(Vector2.zero, position) / (tileSize * (size * 1.5f)), 0, 0.8f);

        // Clear the inbound paths
        foreach (Vector2 line in _roadLines) {
            Vector2 point = FindNearestPointOnLine(Vector2.zero, line * 10f, position);
            float penalty = Mathf.Max(0, 4f - Vector2.Distance(point, position) * 4f) / 3f;
            state -= penalty;
        }

        // Protect turret area
        state = Mathf.Clamp01(state - Mathf.Max(0, turretArea - Vector2.Distance(turret.position, position)));

        return Mathf.Clamp01(state);
    }

    public Vector2? LineLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f) {
            return null;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f) {
            return null;
        }

        Vector2 intersection = Vector2.zero;
        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return intersection;
    }

    public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point) {
        //Get heading
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Vector2 line in _roadLines) {
            Debug.DrawLine(Vector3.zero, line * 5f);
        }
        foreach (Vector2 point in _spawnPoints) {
            Debug.DrawLine(point, point + Vector2.up, Color.red);
            Debug.DrawLine(point, point + Vector2.right, Color.red);
        }
    }

    public Tile GetTile(Vector2 point) {
        Vector2 center = new Vector2(tileSize * (size / 2), tileSize * (size / 2));
        Vector2 position = point + center;
        Vector2 percent = (position / tileSize) / size;
        int x = (int)((1f - Mathf.Clamp01(percent.x)) * size);
        int y = (int)((1f - Mathf.Clamp01(percent.y)) * size);
        return tiles[Mathf.Clamp(x, 0, 63), Mathf.Clamp(y, 0, 63)];
    }

    public List<Tile> GetNeighbours(Tile tile) {
        List<Tile> neighbours = new List<Tile>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;
                int cx = tile.x + x;
                int cy = tile.y + y;
                if (cx >= 0 && cx < size && cy >= 0 && cy < size) {
                    neighbours.Add(tiles[cx, cy]);
                }
            }
        }
        return neighbours;
    }

    private void OnDrawGizmos() {
        Vector2 center = new Vector2(tileSize * (size / 2), tileSize * (size / 2));
        // Gizmos.DrawWireCube(center - new Vector2((size / 2) * tileSize, (size / 2) * tileSize), center + new Vector2((size / 2) * tileSize, (size / 2) * tileSize));
    }
}
