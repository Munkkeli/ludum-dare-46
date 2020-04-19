using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public Map map;
    public float difficultyMultiplier = 1;
    public float waveDuration = 10;

    public GameObject enemy;

    public float _gameDuration = 0;
    private float _waveTimer = 0;
    public Wave _currentWave;

    private int _currentClump;
    private float _clumpSpawnInterval;
    private float _straySpawnInterval;
    private float _clumpSpawnIntervalTimer;
    private float _straySpawnIntervalTimer;

    [System.Serializable]
    public struct Wave {
        public int strayEnemies;
        public int[] clumps;

        public Wave(int strayEnemies, int[] clumps) {
            this.strayEnemies = strayEnemies;
            this.clumps = clumps;
        }
    }

    private Vector2 GetRandomSpawnPoint() {
        return Map.current._spawnPoints[Random.Range(0, Map.current._spawnPoints.Length)];
    }

    private void Update() {
        if (Controller.current.health <= 0) return;

        _gameDuration += Time.deltaTime;

        if (_waveTimer > 0) {
            _waveTimer -= Time.deltaTime;
        } else {
            _waveTimer = waveDuration + _waveTimer;
            _currentWave = GetEnemiesToSpawn();

            _clumpSpawnInterval = waveDuration / _currentWave.clumps.Length;
            _straySpawnInterval = waveDuration / _currentWave.strayEnemies;
            _currentClump = 0;
        }

        if (_clumpSpawnIntervalTimer > 0) {
            _clumpSpawnIntervalTimer -= Time.deltaTime;
        } else {
            _clumpSpawnIntervalTimer = _clumpSpawnInterval + _clumpSpawnIntervalTimer;
            Vector2 point = GetRandomSpawnPoint();
            StartCoroutine(SpawnClump(_currentWave.clumps[_currentClump], point));
            _currentClump++;
        }

        if (_straySpawnIntervalTimer > 0) {
            _straySpawnIntervalTimer -= Time.deltaTime;
        } else {
            _straySpawnIntervalTimer = _straySpawnInterval + _straySpawnIntervalTimer;
            SpawnEnemy(GetRandomSpawnPoint());
        }
    }

    private void SpawnEnemy(Vector2 point) {
        GameObject instance = Instantiate(this.enemy, point + new Vector2((0.5f - Random.value) * 0.5f, (0.5f - Random.value) * 0.5f), Quaternion.identity);
        Enemy enemy = instance.GetComponent<Enemy>();
        enemy.speed = enemy.speed + Random.Range(-enemy.speed * 0.1f, enemy.speed * 0.1f);
        enemy.health = enemy.health + (_gameDuration / 60.5f);
    }

    public Wave GetEnemiesToSpawn() {
        float df = difficultyMultiplier;

        int numberOfEnemiesInAClump = (int)Mathf.Clamp(_gameDuration / (30f / df), 2, 64);

        int minClumps = (int)Mathf.Clamp(_gameDuration / (60f / df), 1, 32);
        int maxClumps = (int)Mathf.Clamp(_gameDuration / (52f / df), 1, 64);
        int numberOfClumpsToSpawn = Random.Range(minClumps, maxClumps);

        int numberOfStrayEnemies = (int)Mathf.Clamp(_gameDuration / (5f / df), 4, 512);

        List<int> clumpList = new List<int>();
        for (int i = 0; i < numberOfClumpsToSpawn; i++) {
            int clumpVariation = (int)Random.Range(-numberOfEnemiesInAClump * 0.05f, numberOfEnemiesInAClump * 0.05f);
            clumpVariation = Mathf.Min(clumpVariation, numberOfStrayEnemies);
            numberOfStrayEnemies -= clumpVariation;
            clumpList.Add(numberOfEnemiesInAClump + clumpVariation);
        }

        return new Wave(numberOfStrayEnemies, clumpList.ToArray());
    }

    public IEnumerator SpawnClump(int number, Vector2 point) {
        int current = 0;
        while (current < number) {
            SpawnEnemy(point);
            current++;
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));
        }
        yield return null;
    }
}
