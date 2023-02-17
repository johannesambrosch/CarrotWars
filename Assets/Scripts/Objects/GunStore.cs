using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStore : MonoBehaviour
{
    public static GunStore instance;
    public int price = 3;
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
                Farmer.instance.gunStoreSelected = true;
            }
            else
            {
                moneyIcon.SetActive(true);
                Farmer.instance.shopLocked.Play();
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
        Farmer.instance.gunStoreSelected = false;
        moneyIcon.SetActive(false);
    }
}
