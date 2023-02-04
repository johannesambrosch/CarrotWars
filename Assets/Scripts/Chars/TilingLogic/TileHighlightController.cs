using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlightController : MonoBehaviour
{
    public int horizontalIndex, verticalIndex;

    public Color suggestedPathColor, lockedPathColor;

    private Image image;
    internal Vector2Int Location => new Vector2Int(horizontalIndex,verticalIndex);

    void Start()
    {
        image = GetComponent<Image>();
        GameManager.instance.RegisterTile(this, horizontalIndex, verticalIndex);
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
        image.color = lockedPath ? lockedPathColor : suggestedPathColor;
    }
}
