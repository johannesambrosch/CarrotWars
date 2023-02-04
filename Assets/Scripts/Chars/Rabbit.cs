using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer highlightSprite;

    public Color HoverColor, SelectColor;

    public int posX, posY;

    private bool selected = false, hovered = false;

    internal List<TileHighlightController> lockedPath, suggestedPath;

    private Coroutine currentMove;

    void Start()
    {
        
    }

    void Update()
    {
        bool highlightVisible = hovered || selected;
        highlightSprite.enabled = highlightVisible;
        highlightSprite.color = selected ? SelectColor : HoverColor;
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

        if (currentMove != null)
            StopCoroutine(currentMove);

        currentMove = StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        yield return null;
    }
}
