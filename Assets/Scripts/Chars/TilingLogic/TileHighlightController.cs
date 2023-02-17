using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlightController : MonoBehaviour
{
    public int horizontalIndex, verticalIndex;

    private Animator animator;

    public Color suggestedPathColor, lockedPathColor;

    private Image image;
    internal Vector2Int Location => new Vector2Int(horizontalIndex, verticalIndex);

    void Awake()
    {
        image = GetComponent<Image>();
        GameManager.instance.RegisterTile(this, horizontalIndex, verticalIndex);
        animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    internal void SetHover(bool hovered)
    {
        image.enabled = hovered;
    }

    internal void SetHighlightState(bool lockedPath)
    {
        animator.SetTrigger(lockedPath ? "select" : "hover");
        image.color = lockedPath ? lockedPathColor : suggestedPathColor;
    }
}
