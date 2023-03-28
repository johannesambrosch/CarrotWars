using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRabbitStore : MonoBehaviour
{
    public List<TileHighlightController> spawnTiles;
    public int price = 2;
    public GameObject selectTile, moneyIcon, spawnPrefab;

    public AudioSource purchaseSound, notEnoughMoneySound;

    protected bool hoveredLastFrame = false;
    protected bool hoveredThisFrame = false;

    protected void LateUpdate()
    {
        if(Input.GetMouseButtonDown(0) && hoveredThisFrame)
        {
            bool canAfford = true;
            if (canAfford)
            {
                OnPurchaseSuccess();
            }
            else
            {
                OnNotEnoughMoney();
            }
        }
        if(hoveredLastFrame && !hoveredThisFrame)
        {
            StopHover();
        }
        else if (hoveredThisFrame)
        {
            hoveredLastFrame = true;
            hoveredThisFrame = false;
        }
    }

    protected void OnNotEnoughMoney()
    {
        notEnoughMoneySound?.Play();
    }

    public virtual void StopHover()
    {
        selectTile.SetActive(false);
        moneyIcon.SetActive(false);
    }

    public virtual void HoverShop()
    {
        selectTile.SetActive(true);
        moneyIcon.SetActive(true);
        hoveredThisFrame = true;
    }
    protected virtual void OnPurchaseSuccess() {
        purchaseSound?.Play();
    }

}
