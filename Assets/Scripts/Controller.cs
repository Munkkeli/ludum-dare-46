using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller current;

    public LayerMask harvesterLayer;
    public GameObject harvester;

    public int wood = 0;
    public int stone = 0;
    public int crystals = 0;
    public int health = 0;
    public int kills = 0;
    public float charge = 0;
    public int requiredCharge = 600;
    public bool start = false;
    public bool startUi = true;
    public bool end = false;
    public bool endDone = false;

    public int towerPrice = 180;
    public int upgradeCost = 100;

    private Transform _movedHarvester;
    private Vector2 _relativePosition;

    private int _gameOverClicks = 0;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (current.health <= 0 && (start || end)) {
            if (Input.GetMouseButtonUp(0)) {
                _gameOverClicks++;
            }
            if (_gameOverClicks >= 5) {
                if (!end) {
                    Application.LoadLevel(0);
                } else {
                    end = false;
                    health = 100;
                    start = true;
                }
                _gameOverClicks = 0;
            }
            return;
        };

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        charge += Time.deltaTime;

        if (charge >= requiredCharge && !endDone) {
            endDone = true;
            start = false;
            end = true;
            current.health = 0;
            return;
        }

        if (Input.GetMouseButtonDown(0) && !startUi) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorldPosition, 0.05f, harvesterLayer);
            if (hits.Length > 0) {
                Collider2D hit = hits[0];
                if (hit.GetComponent<Harvester>() != null) {
                    _movedHarvester = hit.transform;
                    _relativePosition = (Vector2)hit.transform.position - mouseWorldPosition;
                    UI.current.selected = hit.GetComponent<Harvester>();

                    if (!start) {
                        start = true;
                        health = 20;
                    }
                }
            } else {
                UI.current.selected = null;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            _movedHarvester = null;
        }

        if (_movedHarvester != null) {
            _movedHarvester.transform.position = mouseWorldPosition + _relativePosition;
            _movedHarvester.transform.position += new Vector3(0, 0, -1);
        }

        Debug.DrawLine(mouseWorldPosition, mouseWorldPosition + Vector2.up, Color.white);
    }

    public static Harvester CreateHarvester(Vector2 position, float damage, float speed, float size) {
        GameObject instance = Instantiate(Controller.current.harvester, position, Quaternion.identity);
        Harvester harvester = instance.GetComponent<Harvester>();
        harvester.size = size;
        harvester.power = damage;
        harvester.speed = speed;

        return harvester;
    }

    public void Damage() {
        health = Mathf.Max(0, health - 1);
    }
}
