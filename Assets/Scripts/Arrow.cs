using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 1;
    public float damage = 10;
    public AudioClip hitSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed, Space.Self);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Spruce>() != null) {
            collision.gameObject.GetComponent<Spruce>().Hit(damage);
        }
        if (collision.gameObject.GetComponent<Enemy>() != null) {
            collision.gameObject.GetComponent<Enemy>().Hit(damage);
        }

        AudioSource.PlayClipAtPoint(hitSound, transform.position);

        Destroy(gameObject);
    }
}
