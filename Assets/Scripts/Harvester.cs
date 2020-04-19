using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour {
    public enum TowerType {
        Generic,
        Attack,
        Harvest
    }

    public static float lastPlayedChop;
    public static float lastImpactSound;

    public AudioClip[] chopSound;
    public AudioClip fallSound;

    public AudioClip[] hitSounds;
    public AudioClip[] deathSounds;

    public LayerMask treeLayers;
    public LayerMask enemyLayers;
    public float size = 1;
    public float power = 5;
    public float speed = 1;

    public int tier = 1;
    public TowerType type = TowerType.Generic;

    public TextMesh text;

    private List<Spruce> spruces = new List<Spruce>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Tile> tiles = new List<Tile>();

    private Vector2 lastPosition;
    private float _harvestTimer;
    public int lastCost = 0;

    private Material _material;

    private float _powerU = 0.15f;
    private float _speedU = 0.07f;
    private float _sizeU = 0.07f;

    private AudioSource _audioSource;

    public void Upgrade() {
        tier++;

        power = (power + (power * _powerU * tier));
        speed = (speed + (speed * _speedU * tier));
        size = (size + (size * _sizeU * tier));

        transform.localScale = new Vector3(size, size, 1);

        text.text = $"lvl {tier}";
        text.gameObject.SetActive(tier > 1);
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        lastCost = Controller.current.upgradeCost;
        _material = GetComponent<MeshRenderer>().material;
        transform.localScale = new Vector3(size, size, 1);
        text.gameObject.SetActive(false);
        _harvestTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.current.health <= 0) return;

        CheckForEnemies();

        if (lastPosition != (Vector2)transform.position) {
            CheckForTiles();
            CheckForTrees();
            lastPosition = transform.position;
        }

        if (_harvestTimer > 0) {
            _harvestTimer -= Time.deltaTime;
        } else {
            _harvestTimer = (1f / speed) + _harvestTimer;

            if (enemies.Count > 0) {
                Enemy current = enemies[Random.Range(0, enemies.Count)];

                current.Hit(power);

                if (current.health <= 0 || current.gameObject == null) {
                    if (Time.time - lastImpactSound > 0.1f) {
                        _audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)], 0.5f);
                        lastImpactSound = Time.time;
                    }
                } else {
                    if (Time.time - lastImpactSound > 0.1f) {
                        _audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)], 0.3f);
                        lastImpactSound = Time.time;
                    }
                }
                // Controller.current.crystals += 1;
            } else if (spruces.Count > 0) {
                Spruce current = spruces[Random.Range(0, spruces.Count)];

                // Failsafe
                if (!current) {
                    spruces.Remove(current);
                    return;
                }

                int wood = (int)current.Hit(power);
                Controller.current.wood += wood;

                if (current.health <= 0 || current.gameObject == null) {
                    spruces.Remove(current);

                    if (Time.time - lastPlayedChop > 0.05f) {
                        AudioSource.PlayClipAtPoint(fallSound, current.transform.position, 0.5f);
                        lastPlayedChop = Time.time;
                    }
                } else {
                    if (Time.time - lastPlayedChop > 0.05f) {
                        AudioSource.PlayClipAtPoint(chopSound[Random.Range(0, chopSound.Length)], current.transform.position, 0.25f);
                        lastPlayedChop = Time.time;
                    }
                }
            }
        }

        if (UI.current.selected == this) {
            _material.SetFloat("_Pulse", 1);
        } else {
            _material.SetFloat("_Pulse", 0);
        }
    }

    private void CheckForTrees() {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, size / 2f, treeLayers);

        spruces.Clear();
        foreach (Collider2D collider in hitColliders) {
            spruces.Add(collider.gameObject.GetComponent<Spruce>());
        }
    }

    private void CheckForEnemies() {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, size / 2f, enemyLayers);

        enemies.Clear();

        foreach (Collider2D collider in hitColliders) {
            if (collider.gameObject.GetComponent<Enemy>().health > 0) {
                enemies.Add(collider.gameObject.GetComponent<Enemy>());
            }
        }
    }

    private void CheckForTiles() {
        foreach (Tile tile in tiles) {
            tile.isUnderHarvester = false;
        }

        tiles.Clear();

        int size = (int)(this.size / Map.current.tileSize);
        for (int x = -size; x < size; x++) {
            for (int y = -size; y < size; y++) {
                float cx = (int)(transform.position.x / Map.current.tileSize) * Map.current.tileSize;
                float cy = (int)(transform.position.y / Map.current.tileSize) * Map.current.tileSize;

                Vector2 position = new Vector2(cx, cy) + new Vector2(x * Map.current.tileSize, y * Map.current.tileSize)+ new Vector2(Map.current.tileSize / 2, Map.current.tileSize / 2);
                if (Vector2.Distance(position, transform.position) > size * Map.current.tileSize) continue;
                Tile tile = Map.current.GetTile(position);
                tile.isUnderHarvester = true;
                if (!tiles.Contains(tile)) tiles.Add(tile);
            }
        }
    }

    public string GetUpgradeMessage() {
        string message = $"Upgrade beam ({GetUpgradeCost()}) wood";

        if (type == TowerType.Generic) {
            message += $"\n+{(power * _powerU * tier).ToString("F2")} power";
            message += $"\n+{(speed * _speedU * tier).ToString("F2")} speed";
        }


        return message;
    }

    public int GetUpgradeCost() {
        return (int)(tier * (tier * 2.65f) * 50);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x / 2f);

        if (tiles != null) {
            foreach (Tile tile in tiles) {
                Gizmos.color = Color.white;
                if (tile.weight > 0) Gizmos.color = Color.red;
                Gizmos.DrawCube(tile.position, Vector2.one * Map.current.tileSize);
            }
        }
    }
}
