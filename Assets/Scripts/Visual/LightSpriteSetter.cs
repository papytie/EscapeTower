using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSpriteSetter : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Light2D light2D;
    private Sprite currentSprite;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        currentSprite = spriteRenderer.sprite;
        light2D.lightCookieSprite = currentSprite;

    }
}
