using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Rabbit hoveredRabbit;
    private Rabbit selectedRabbit;

    public static GameManager instance;

    private TileHighlightController[,] tileGrid;

    void Awake()
    {
        instance = this;
        tileGrid = new TileHighlightController[22,14];
    }

    void Update()
    {
        UpdateRabbitHover();

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }

        
    }

    internal void RegisterTile(TileHighlightController tile, int horizontalIndex, int verticalIndex)
    {
        tileGrid[horizontalIndex, verticalIndex] = tile;
    }

    private void HandleRightClick()
    {
        throw new NotImplementedException();
    }

    private void HandleLeftClick()
    {
        if (hoveredRabbit != null)
        {
            SelectRabbit();
        }
        else
        {
            DeselectRabbit();
        }
    }

    private void UpdateRabbitHover()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Rabbit"))
        {
            HoverRabbit(hit.collider.GetComponent<Rabbit>());
        }
        else
        {
            UnhoverRabbit();
        }
    }

    public void HoverRabbit(Rabbit rabbit)
    {
        UnhoverRabbit();
        rabbit.SetHoverState(true);
        hoveredRabbit = rabbit;
    }

    private void UnhoverRabbit()
    {
        if (hoveredRabbit == null)
            return;

        hoveredRabbit.SetHoverState(false);
        hoveredRabbit = null;
    }
    private void SelectRabbit()
    {
        DeselectRabbit();
        selectedRabbit = hoveredRabbit;
        selectedRabbit.SetSelectState(true);
    }

    private void DeselectRabbit()
    {
        if (selectedRabbit == null)
            return;

        selectedRabbit.SetSelectState(false);
        selectedRabbit = null;
    }
}
