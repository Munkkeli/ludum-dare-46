using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI current;

    public LayerMask uiLayer;

    public TextMesh woodText;
    public TextMesh healthText;
    public TextMesh killsText;
    public TextMesh gameOverKillsText;
    public TextMesh chargeText;

    public GameObject upgradeButton;
    public GameObject attackButton;
    public GameObject harvestButton;
    public GameObject createButton;

    public GameObject[] guide;
    public GameObject[] gameOver;
    public GameObject[] endScreen;

    public Harvester selected;

    void Start()
    {
        foreach (GameObject item in guide) {
            item.SetActive(true);
        }

        foreach (GameObject item in gameOver) {
            item.SetActive(false);
        }

        foreach (GameObject item in endScreen) {
            item.SetActive(false);
        }

        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        woodText.text = $"Wood: {Controller.current.wood}";
        healthText.text = $"Health: {(Controller.current.start ? Controller.current.health : 100)}";
        killsText.text = $"Kills: {Controller.current.kills}";
        gameOverKillsText.text = $"Kills: {Controller.current.kills}";
        chargeText.text = $"Charge: {(int)Controller.current.charge}/{Controller.current.requiredCharge}";

        if (!Controller.current.start && guide[0].activeSelf && Input.GetMouseButtonUp(0)) {
            Controller.current.startUi = false;
            foreach (GameObject item in guide) {
                item.SetActive(false);
            }
        }

        if (Controller.current.start && Controller.current.health <= 0 && !gameOver[0].activeSelf ) {
            foreach (GameObject item in gameOver) {
                item.SetActive(true);
            }
        }

        if (Controller.current.end && !endScreen[0].activeSelf) {
            foreach (GameObject item in endScreen) {
                item.SetActive(true);
            }
        }

        if (!Controller.current.end && Controller.current.endDone && endScreen[0].activeSelf) {
            foreach (GameObject item in endScreen) {
                item.SetActive(false);
            }
        }

        createButton.GetComponentInChildren<TextMesh>().text = $"Create new beam\nWood: {Controller.current.towerPrice}";

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonUp(0)) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorldPosition, 0.05f, uiLayer);
            if (hits.Length > 0) {
                Collider2D hit = hits[0];

                if (hit.gameObject == createButton && Controller.current.wood >= Controller.current.towerPrice) {
                    Controller.current.wood -= Controller.current.towerPrice;
                    Controller.current.towerPrice = (int)(Controller.current.towerPrice * 1.5f);

                    Controller.CreateHarvester(Vector2.zero, 5, 1, 1);
                }

                if (selected) {
                    int cost = selected.GetUpgradeCost();
                    if (hit.gameObject == upgradeButton && Controller.current.wood >= cost) {
                        Controller.current.wood -= cost;
                        selected.lastCost = cost;
                        selected.Upgrade();
                    }
                }
                
            }
        }

        if (selected == null) {
            upgradeButton.SetActive(false);
            attackButton.SetActive(false);
            harvestButton.SetActive(false);
            createButton.SetActive(true);
        } else {
            upgradeButton.GetComponentInChildren<TextMesh>().text = selected.GetUpgradeMessage();
            upgradeButton.SetActive(true);
        }
    }
}
