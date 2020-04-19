using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> enemyList = new List<Enemy>();

    public float health = 60;
    public Vector2[] path;
    public float speed = 0.5f;
    public Color tintColor;

    int targetIndex = 0;

    private SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();

        enemyList.Add(this);

        PathRequestManager.RequestPath(transform.position, Vector2.zero, gameObject, OnPathFound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy() {
        StopCoroutine("FollowPath");
        StopCoroutine("DamageTint");
    }

    public void Hit(float amount) {
        health -= amount;
        if (health < 0) {
            enemyList.Remove(this);
            Controller.current.kills++;
            Destroy(gameObject);
        } else {
            StartCoroutine(DamageTint());
        }
    }

    public void OnPathFound(Vector2[] path, bool isSuccess) {
        if (isSuccess && gameObject != null) {
            this.path = path;
            StopCoroutine("FollowPath");
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath() {
        Vector2 currentWaypoint = path[0];

        while (true) {
            if (Vector2.Distance(transform.position, currentWaypoint) < 0.1f) {
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
            if (_renderer == null) yield break;

            tintTimer -= Time.deltaTime;

            _renderer.color = Color.Lerp(Color.white, tintColor, tintTimer / tintDuration);

            yield return null;
        }

        if (_renderer == null) _renderer.color = Color.white;

        yield break;
    }
}
