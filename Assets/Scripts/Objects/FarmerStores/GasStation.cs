using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasStation : MonoBehaviour
{
    public static GasStation instance;
    public int price = 5;
    public GameObject selectTile, moneyIcon;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Farmer"))
        {
            bool canBuy = Farmer.instance.carrotCount >= price;
            if (canBuy)
            {
                selectTile.SetActive(true);
                Farmer.instance.gasStationSelected = true;
            }
            else
            {
                moneyIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Farmer"))
        {
            DisableShop();
        }
    }

    public void DisableShop()
    {
        selectTile.SetActive(false);
        Farmer.instance.gasStationSelected = false;
        moneyIcon.SetActive(false);
    }
}
