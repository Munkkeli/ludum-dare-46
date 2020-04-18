using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> enemyList = new List<Enemy>();

    public float health = 60;
    public Vector2[] path;
    public float speed = 0.5f;
    public AudioClip deathSound;

    int targetIndex = 0;

    private SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();

        StartCoroutine(FollowPath());

        enemyList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(float amount) {
        health -= amount;
        if (health < 0) {
            enemyList.Remove(this);
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            Destroy(gameObject);
        }
        StartCoroutine(DamageTint());
    }
        

    IEnumerator FollowPath() {
        Vector2 currentWaypoint = path[0];

        while (true) {
            if ((Vector2)transform.position == currentWaypoint) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    yield break;
                }
                currentWaypoint = path[targetIndex];

            }

            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator DamageTint() {
        float tintDuration = 0.2f;
        float tintTimer = tintDuration;

        while (tintTimer > 0) {
            tintTimer -= Time.deltaTime;

            _renderer.color = Color.Lerp(Color.white, Color.red, tintTimer / tintDuration);

            yield return null;
        }

        _renderer.color = Color.white;

        yield break;
    }
}
