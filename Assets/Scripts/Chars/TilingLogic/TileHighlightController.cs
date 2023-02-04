using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlightController : MonoBehaviour
{
    public int horizontalIndex, verticalIndex;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        GameManager.instance.RegisterTile(this, horizontalIndex, verticalIndex);
    }

    void Update()
    {
        
    }

    internal void SetHighlight(bool highlight)
    {
        image.enabled = highlight;
    }
}
