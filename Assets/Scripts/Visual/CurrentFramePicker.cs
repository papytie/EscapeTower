using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentFramePicker : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite currentSprite;
    public Rect spriteRect;
    public Vector2 spritePivot;


    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = spriteRenderer.sprite;
        spriteRect = spriteRenderer.sprite.rect;
        spritePivot = spriteRenderer.sprite.pivot;
    }

    private void Update()
    {
        if (currentSprite != spriteRenderer.sprite)
        {
            //Debug.Log("Sprite changed from " + currentSprite + " to " + spriteRenderer.sprite);
            currentSprite = spriteRenderer.sprite;
        }

        if(spriteRect != spriteRenderer.sprite.rect)
        {
            spriteRect = spriteRenderer.sprite.rect;
        }

        if(spritePivot != spriteRenderer.sprite.pivot) 
        {
            spritePivot = spriteRenderer.sprite.pivot;
        }
        
    }
}
