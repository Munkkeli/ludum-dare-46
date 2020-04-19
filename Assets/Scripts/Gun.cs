using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public LayerMask enemyLayer;
    public GameObject projectile;
    public Transform gun;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // convert mouse position into world coordinates
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get direction you want to point at
        Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;

        // set vector of transform directly
        gun.up = direction;

        /*
        if (Input.GetMouseButtonDown(0)) {
            GameObject instance = Instantiate(projectile, transform.position, gun.rotation);
            instance.GetComponent<Arrow>().damage = 1000;
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer))) {
            Controller.current.Damage();
            Destroy(collision.gameObject);
        }
    }
}
