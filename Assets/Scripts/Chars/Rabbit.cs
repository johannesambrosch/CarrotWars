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

    public Vector2Int location;

    internal List<TileHighlightController> lockedPath, suggestedPath;

    public States currentState = States.idle;

    public float timePerFieldMove = 1f;
    public float eatTime = 1f;

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


    void Start()
    {

    }

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

        GetComponent<BoxCollider2D>().enabled = false;

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }
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
            moveStartTile = location;
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
                    location = moveTargetTile;
                }
                yield return null;
            }
            lockedPath.RemoveAt(0);

            var carrot = GameManager.instance.carrotGrid[location.x, location.y];
            if (carrot != null)
            {
                currentState = States.eat;
                animator.SetTrigger("eat");
                yield return new WaitForSeconds(eatTime);
                carrot.Remove();
            }
        }
        currentState = States.idle;
        animator.SetTrigger("idle");
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
}
