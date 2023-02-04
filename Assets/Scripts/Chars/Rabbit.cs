using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    public SpriteRenderer highlightSprite;

    public Color HoverColor, SelectColor;

    private bool selected = false, hovered = false;

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
}
