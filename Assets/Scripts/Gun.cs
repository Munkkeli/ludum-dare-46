using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject projectile;

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
        transform.up = direction;

        if (Input.GetMouseButtonDown(0)) {
            Instantiate(projectile, transform.position, transform.rotation);
        }
    }
}
