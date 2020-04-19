using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spruce : MonoBehaviour
{
    public float health = 1000;
    public Color hitColor = Color.red;
    public Tile tile;

    private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float Hit(float amount) {
        if (gameObject == null) return 0;

        StartCoroutine(DamageTint());
        float damageDealt = Mathf.Min(amount, health);
        health -= damageDealt;

        if (health <= 0) {
            if (tile != null) tile.isWalkable = true;
            Destroy(gameObject);
        }

        return damageDealt;
    }

    private void OnDestroy() {
        StopCoroutine("DamageTint");
    }

    public IEnumerator DamageTint() {
        float tintDuration = 0.2f;
        float tintTimer = tintDuration;

        while (tintTimer > 0) {
            tintTimer -= Time.deltaTime;

            renderer.color = Color.Lerp(Color.white, hitColor, tintTimer / tintDuration);

            yield return null;
        }

        renderer.color = Color.white;

        yield break;
    }
}
