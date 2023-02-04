using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject CarrotPrefab;
    public int carrotCount = 16;

    internal Rabbit hoveredRabbit;
    internal Rabbit selectedRabbit;

    private bool RabbitIsSelected => selectedRabbit != null;

    public static GameManager instance;

    private TileHighlightController[,] tileGrid;
    internal Carrot[,] carrotGrid;

    private const int tileCountX = 18;
    private const int tileCountY = 14;

    void Awake()
    {
        instance = this;
        tileGrid = new TileHighlightController[tileCountX, tileCountY];
    }

    private void Start()
    {
        InitCarrotGrid();
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

        UpdatePathTiles();
    }

    private void InitCarrotGrid()
    {
        carrotGrid = new Carrot[tileCountX, tileCountY];

        for (int i = 0; i < carrotCount; i++)
        {

            int carrotX, carrotY;
            do
            {
                carrotX = Random.Range(0, 8);
                carrotY = Random.Range(0, 8);

                if (carrotX >= 4) carrotX += 2;
                if (carrotY >= 4) carrotY += 2;

                carrotX += 2;
                carrotY += 2;

            } while (carrotGrid[carrotX, carrotY] != null);

            GameObject newCarrot = Instantiate(CarrotPrefab);
            var carrotBehavior = newCarrot.GetComponent<Carrot>();
            carrotBehavior.SetLocation(carrotX, carrotY);

            newCarrot.transform.position = tileGrid[carrotX, carrotY].transform.position;

            carrotGrid[carrotX, carrotY] = carrotBehavior;
        }
    }

    private void UpdatePathTiles()
    {
        ClearTileHighlight();

        if (!RabbitIsSelected) return;

        bool suggestionFound = false;
        selectedRabbit.suggestedPath = null;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Tile"))
        {
            int rabbitX = selectedRabbit.location.x;
            int rabbitY = selectedRabbit.location.y;
            var targetTile = hit.collider.GetComponent<TileHighlightController>();
            int targetX = targetTile.horizontalIndex;
            int targetY = targetTile.verticalIndex;

            if (rabbitX != targetX || rabbitY != targetY)
            {
                suggestionFound = true;
                selectedRabbit.suggestedPath = new List<TileHighlightController>();
                int xDif = targetX - rabbitX;
                int yDif = targetY - rabbitY;

                bool horizontalFirst = Mathf.Abs(xDif) > Mathf.Abs(yDif);

                int curX = rabbitX;
                int curY = rabbitY;

                if (horizontalFirst)
                {
                    while (curX != targetX)
                    {
                        if (xDif >= 0)
                            curX++;
                        else
                            curX--;

                        selectedRabbit.suggestedPath.Add(tileGrid[curX, curY]);
                    }
                    while (curY != targetY)
                    {
                        if (yDif >= 0)
                            curY++;
                        else
                            curY--;

                        selectedRabbit.suggestedPath.Add(tileGrid[curX, curY]);
                    }
                }
                else
                {
                    while (curY != targetY)
                    {
                        if (yDif >= 0)
                            curY++;
                        else
                            curY--;

                        selectedRabbit.suggestedPath.Add(tileGrid[curX, curY]);
                    }
                    while (curX != targetX)
                    {
                        if (xDif >= 0)
                            curX++;
                        else
                            curX--;

                        selectedRabbit.suggestedPath.Add(tileGrid[curX, curY]);
                    }
                }
            }
        }

        if (selectedRabbit.lockedPath != null && selectedRabbit.lockedPath.Count > 0)
        {
            HighlightPath(selectedRabbit.lockedPath, true);
        }

        //check if we want to also display a path suggestion
        bool displaySuggestion = suggestionFound;

        if (
            selectedRabbit.lockedPath != null
            && selectedRabbit.suggestedPath != null
            && selectedRabbit.lockedPath.Count > 0)
        {
            //if the hovered tile is already the locked target, no need to display suggestion path
            var lockedTarget = selectedRabbit.lockedPath[selectedRabbit.lockedPath.Count - 1];
            var suggestedTarget = selectedRabbit.suggestedPath[selectedRabbit.suggestedPath.Count - 1];

            if (lockedTarget == suggestedTarget) displaySuggestion = false;
        }

        if (displaySuggestion)
        {
            HighlightPath(selectedRabbit.suggestedPath, false);
        }
    }

    private void ClearTileHighlight()
    {
        for (int i = 0; i < tileCountX; i++)
        {
            for (int j = 0; j < tileCountY; j++)
            {
                tileGrid[i, j].SetHover(false);
            }
        }
    }

    private void HighlightPath(List<TileHighlightController> path, bool locked)
    {
        for (int i = 0; i < path.Count; i++)
        {
            path[i].SetHighlightState(locked);
            path[i].SetHover(true);
        }
    }

    internal void RegisterTile(TileHighlightController tile, int horizontalIndex, int verticalIndex)
    {
        tileGrid[horizontalIndex, verticalIndex] = tile;
    }

    private void HandleRightClick()
    {
        if (RabbitIsSelected && selectedRabbit.suggestedPath != null)
        {
            selectedRabbit.CommandMove();
        }
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
        int layerMask = LayerMask.GetMask("Rabbit");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);

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

    internal void UnhoverRabbit()
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

    internal void DeselectRabbit()
    {
        if (selectedRabbit == null)
            return;

        selectedRabbit.SetSelectState(false);
        selectedRabbit = null;
    }
}
