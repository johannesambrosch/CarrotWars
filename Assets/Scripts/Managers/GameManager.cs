using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject CarrotPrefab, RabbitPrefab;
    public int totalCarrotCount = 16;
    public int totalRabbitCount = 6;

    public GameObject stateGame, stateFarmerRoundWin, stateRabbitRoundWin, stateProgressBar;

    public float roundEndDuration = 10f, preparationDuration = 20f;

    public GameObject CarrotContainer, RabbitContainer;

    public int livesFarmer = 3, livesRabbits = 3;

    public enum States
    {
        Preparation,
        Game,
        FarmerRoundWin,
        RabbitRoundWin
    }

    public States currentState = States.Preparation;

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
        ChangeStates(States.Preparation);
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

    private void ChangeStates(States state)
    {
        currentState = state;
        DeactivateStates();

        switch (state)
        {
            case States.Preparation:
                //stateGame.SetActive(true);
                stateProgressBar.SetActive(true);
                InitCarrotGrid();
                SetMainStageTilesActiveState(false);
                InitRabbits();
                StartCoroutine(PreparationTimerCoroutine());
                break;
            case States.Game:
                SetMainStageTilesActiveState(true);
                //stateGame.SetActive(true);
                break;
            case States.FarmerRoundWin:
                stateFarmerRoundWin.SetActive(true);
                stateProgressBar.SetActive(true);
                break;
            case States.RabbitRoundWin:
                stateRabbitRoundWin.SetActive(true);
                stateProgressBar.SetActive(true);
                break;
            default:
                //stateGame.SetActive(true);
                break;
        }
    }

    private void SetMainStageTilesActiveState(bool activeState)
    {
        for (int i = 0; i < tileCountX - 4; i++)
        {
            for (int j = 0; j < tileCountY; j++)
            {
                tileGrid[i, j].GetComponent<BoxCollider2D>().enabled = activeState;
            }
        }
    }

    private void DeactivateStates()
    {
        //stateGame.SetActive(false);
        stateFarmerRoundWin.SetActive(false);
        stateRabbitRoundWin.SetActive(false);
        stateProgressBar.SetActive(false);
    }

    private void InitCarrotGrid()
    {
        carrotGrid = new Carrot[tileCountX, tileCountY];

        for (int i = 0; i < totalCarrotCount; i++)
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

            GameObject newCarrot = Instantiate(CarrotPrefab, CarrotContainer.transform);
            var carrotBehavior = newCarrot.GetComponent<Carrot>();
            carrotBehavior.SetLocation(carrotX, carrotY);

            newCarrot.transform.position = tileGrid[carrotX, carrotY].transform.position;

            carrotGrid[carrotX, carrotY] = carrotBehavior;
        }
    }

    private void InitRabbits()
    {
        if (Rabbit.allRabbits == null)
            Rabbit.allRabbits = new List<Rabbit>();

        var rabbitGrid = new Rabbit[tileCountX, tileCountY];

        for (int i = Rabbit.allRabbits.Count; i < totalRabbitCount; i++)
        {

            int rabbitX, rabbitY;
            do
            {
                rabbitX = Random.Range(0, 3);
                rabbitY = Random.Range(0, 14);

                rabbitX += 14;

            } while (rabbitGrid[rabbitX, rabbitY] != null);

            GameObject newRabbit = Instantiate(RabbitPrefab, RabbitContainer.transform);
            var rabbitBehavior = newRabbit.GetComponent<Rabbit>();
            Rabbit.allRabbits.Add(rabbitBehavior);
            rabbitBehavior.SetLocation(rabbitX, rabbitY);
            rabbitBehavior.initialLocation = new Vector2Int(rabbitX, rabbitY);

            newRabbit.transform.position = tileGrid[rabbitX, rabbitY].transform.position;

            rabbitGrid[rabbitX, rabbitY] = rabbitBehavior;
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
            int rabbitX = selectedRabbit.Location.x;
            int rabbitY = selectedRabbit.Location.y;
            var targetTile = hit.collider.GetComponent<TileHighlightController>();
            int targetX = targetTile.horizontalIndex;
            int targetY = targetTile.verticalIndex;
            suggestionFound = CheckForPathSuggestion(rabbitX, rabbitY, targetX, targetY);
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

    private bool CheckForPathSuggestion(int rabbitX, int rabbitY, int targetX, int targetY)
    {
        var suggestionFound = false;
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

        return suggestionFound;
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

    public void OnFarmerRoundWin()
    {
        livesRabbits--;
        if(livesRabbits == 0)
        {
            SceneManager.LoadScene("FarmerVictory");
        }
        Farmer.instance.DoReset();

        //Todo: count left carrots
        Debug.Log(Carrot.allCarrots.Count);

        while(Carrot.allCarrots.Count > 0)
        {
            Carrot c = Carrot.allCarrots[0];
            c.Remove(true);
        }

        ChangeStates(States.FarmerRoundWin);
        StartCoroutine(RoundEndTimerCoroutine());
    }

    public void OnRabbitRoundWin()
    {
        livesFarmer--;
        if (livesFarmer == 0)
        {
            SceneManager.LoadScene("RabbitVictory");
        }
        Farmer.instance.DoReset();
        ResetLivingRabbits();

        if (selectedRabbit != null) DeselectRabbit();
        if (hoveredRabbit != null) UnhoverRabbit();

        ChangeStates(States.RabbitRoundWin);
        StartCoroutine(RoundEndTimerCoroutine());
    }

    public void ResetLivingRabbits()
    {
        if (Rabbit.allRabbits == null || Rabbit.allRabbits.Count == 0)
            return;

        foreach (Rabbit rabbit in Rabbit.allRabbits)
        {
            /*HoverRabbit(rabbit);
            SelectRabbit();
            bool pathFound = CheckForPathSuggestion(rabbit.Location.x, rabbit.Location.y, rabbit.initialLocation.x, rabbit.initialLocation.y);
            if (pathFound) rabbit.CommandMove();*/
            rabbit.SetLocation(rabbit.initialLocation.x, rabbit.initialLocation.y);
            rabbit.transform.position = tileGrid[rabbit.initialLocation.x, rabbit.initialLocation.y].transform.position;
        }
    }

    private IEnumerator PreparationTimerCoroutine()
    {
        float startTime = Time.time;
        float targetTime = startTime + preparationDuration;

        while (Time.time < targetTime)
        {
            yield return null;
            float progressBarWidth = Mathf.InverseLerp(targetTime, startTime, Time.time);

            //TODO: update progressBar
            Debug.Log(progressBarWidth);
        }

        ChangeStates(States.Game);
    }

    private IEnumerator RoundEndTimerCoroutine()
    {
        float startTime = Time.time;
        float targetTime = startTime + roundEndDuration;

        while (Time.time < targetTime)
        {
            yield return null;
            float progressBarWidth = Mathf.InverseLerp(targetTime, startTime, Time.time);

            //TODO: update progressBar
            Debug.Log(progressBarWidth);
        }

        ChangeStates(States.Preparation);
    }
}
