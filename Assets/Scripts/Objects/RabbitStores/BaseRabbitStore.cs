using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseRabbitStore : MonoBehaviour
{
    public List<TileHighlightController> spawnTiles;
    public int price = 2;
    public GameObject selectTile, moneyIcon, spawnPrefab;

    public AudioSource purchaseSound, notEnoughMoneySound;

    bool canAfford => Rabbit.rabbitPoints >= price;

    protected bool hoveredLastFrame = false;
    protected bool hoveredThisFrame = false;

    protected void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && hoveredThisFrame)
        {
            if (canAfford)
            {
                OnPurchaseSuccess();
            }
            else
            {
                OnNotEnoughMoney();
            }
        }
        if (hoveredLastFrame && !hoveredThisFrame)
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
        if (notEnoughMoneySound != null)
        {
            notEnoughMoneySound.Play();
        }
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
    protected virtual void OnPurchaseSuccess()
    {
        if(purchaseSound != null)
        {
            purchaseSound.Play();
        }
        Rabbit.rabbitPoints -= price;
        GameManager.instance.UpdateRabbitPointDisplay();
        var newRabbitObject = Instantiate(spawnPrefab);
        var newRabbit = newRabbitObject.GetComponent<Rabbit>();
        newRabbit.isSpawnAnim = true;
        newRabbit.SetLocation(newRabbit.initialLocation.x, newRabbit.initialLocation.y);
        newRabbit.suggestedPath = spawnTiles.ToArray().ToList();
        newRabbit.CommandMove();
    }

}
