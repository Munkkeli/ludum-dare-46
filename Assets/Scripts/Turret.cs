using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 10;
    public float speed = 1;
    public Transform gun;
    public GameObject projectile;
    public AudioClip fireSound;

    private Harvester harvester;
    private Enemy _currentTarget;
    private AudioSource _audioSource;

    private float _fireTimer = 0;

    void Start()
    {
        _fireTimer = speed;
        _audioSource = GetComponent<AudioSource>();

        harvester = Controller.CreateHarvester(transform.position, 5, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (_currentTarget == null) {
            foreach (Enemy enemy in Enemy.enemyList) {
                if (Vector2.Distance(transform.position, enemy.transform.position) < range) {
                    _currentTarget = enemy;
                    break;
                }
            }
        } else {
            // Look at the enemy
            Vector2 direction = ((Vector2)_currentTarget.transform.position - (Vector2)transform.position).normalized;
            gun.up = direction;
        }

        if (_fireTimer > 0) {
            _fireTimer -= Time.deltaTime;
        } else if (_currentTarget != null) {
            _fireTimer = speed - _fireTimer;
            Instantiate(projectile, transform.position, gun.rotation);
            _audioSource.PlayOneShot(fireSound);
        }
        */
    }
}
