using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    // public

    public Animator animator;
    public SpriteRenderer highlightSprite;

    public Color HoverColor, SelectColor;

    public static List<Rabbit> allRabbits;
    public static int rabbitPoints = 0;

    public Vector2Int Location { get; private set; }

    public Vector2Int initialLocation;

    internal List<TileHighlightController> lockedPath, suggestedPath;

    public States currentState = States.idle;

    public float timePerFieldMove = 1f;
    public float eatTime = 1f;

    internal bool isSpawnAnim = false;

    public AudioSource eatSound, dieSound;

    public enum States
    {
        idle,
        moveLeft,
        moveUp,
        moveRight,
        moveDown,
        eat,
        smacked,
        shot,
        runOver,
        win
    }

    //private

    private bool selected = false, hovered = false;
    private Vector2Int moveStartTile, moveTargetTile;
    private Coroutine currentMoveCoroutine;
    

    void Update()
    {
        bool highlightVisible = hovered || selected;
        highlightSprite.enabled = highlightVisible;
        highlightSprite.color = selected ? SelectColor : HoverColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shovel"))
        {
            DisableMe();

            currentState = States.smacked;
            animator.SetTrigger("smack");
        }

        if (collision.CompareTag("Tractor"))
        {
            DisableMe();

            currentState = States.runOver;
            animator.SetTrigger("smack");
        }
    }

    private void DisableMe()
    {
        if (GameManager.instance.selectedRabbit == this)
        {
            GameManager.instance.DeselectRabbit();
        }
        if (GameManager.instance.hoveredRabbit == this)
        {
            GameManager.instance.UnhoverRabbit();
        }

        dieSound.Play();

        GetComponent<BoxCollider2D>().enabled = false;
        allRabbits.Remove(this);

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }

        if(allRabbits.Count == 0)
        {
            GameManager.instance.OnFarmerRoundWin();
        }
    }

    public void SetLocation(int x, int y)
    {
        Location = new Vector2Int(x, y);
    }

    public void GetShot()
    {
        DisableMe();

        currentState = States.shot;
        animator.SetTrigger("shot");
    }

    internal void SetHoverState(bool hovered)
    {
        this.hovered = hovered;
    }

    internal void SetSelectState(bool selected)
    {
        this.selected = selected;
    }

    internal void CommandMove()
    {
        lockedPath = suggestedPath;

        if (currentMoveCoroutine != null)
            StopCoroutine(currentMoveCoroutine);

        currentMoveCoroutine = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while (lockedPath.Count > 0)
        {
            moveStartTile = Location;
            moveTargetTile = lockedPath[0].Location;
            Vector3 initialPos = transform.position;
            Vector3 targetPos = lockedPath[0].transform.position;
            Vector2 moveDirection = moveTargetTile - moveStartTile;
            UpdateState(moveDirection);
            float moveStartTime = Time.time;
            float moveTargetTime = Time.time + timePerFieldMove;
            while (Time.time < moveTargetTime)
            {
                float moveProgress = Mathf.InverseLerp(moveStartTime, moveTargetTime, Time.time);

                transform.position = Vector3.Lerp(initialPos, targetPos, moveProgress);
                if (moveProgress > 0.5f)
                {
                    Location = moveTargetTile;
                }
                yield return null;
            }
            lockedPath.RemoveAt(0);

            if (!isSpawnAnim)
            {
                var carrot = GameManager.instance.carrotGrid[Location.x, Location.y];
                if (carrot != null)
                {
                    currentState = States.eat;
                    animator.SetTrigger("eat");
                    eatSound.Play();
                    yield return new WaitForSeconds(eatTime);
                    if (carrot != null)
                    {
                        carrot.Remove();
                    }
                }
            }
        }
        currentState = States.idle;
        animator.SetTrigger("idle");
        if (isSpawnAnim)
        {
            isSpawnAnim = false;
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void UpdateState(Vector2 moveDirection)
    {
        if (moveDirection.y == 0)
        {
            if (moveDirection.x > 0)
                currentState = States.moveRight;
            else
                currentState = States.moveLeft;
        }
        else
        {
            if (moveDirection.y > 0)
                currentState = States.moveDown;
            else
                currentState = States.moveUp;
        }
        animator.SetTrigger(currentState.ToString());
    }

    internal static void SetPoints(int count)
    {
        rabbitPoints += count;
        if (rabbitPoints > 8)
        {
            rabbitPoints = 8;
        }
    }
}
