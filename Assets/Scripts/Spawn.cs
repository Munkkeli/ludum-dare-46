using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float spawnInterval = 2;
    public Transform target;
    public GameObject enemy;

    private float spawnTimer = 0;

    // Start is called before the first frame update
    void Start() {
        spawnTimer = spawnInterval;

        if (target == null) {
            target = GameObject.Find("Turret").transform;
        }
    }

    // Update is called once per frame
    void Update() {
        if (spawnTimer > 0) {
            spawnTimer -= Time.deltaTime;
        } else {
            spawnTimer = spawnInterval + spawnTimer;
            GameObject instance = Instantiate(enemy, transform.position, Quaternion.identity);
        }
    }
}
