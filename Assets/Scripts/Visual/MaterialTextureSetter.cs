using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static Unity.Collections.AllocatorManager;

public class MaterialTextureSetter : MonoBehaviour
{

  
    public SpriteSecondaryTextureReflection spriteSecondary;
    public CurrentFramePicker currentFramePicker;
    public Vector2 spritePositionOnTexture;
    public SpriteRenderer mySpriteRenderer;
    public SpriteRenderer refSpriteRenderer;

    private Sprite currentSprite;

    //[SerializeField] string fogMaskName = null;
    //[SerializeField] string spritePivotName = null;
    //[SerializeField] Material[] materials = null;

    [SerializeField] Texture2D fogTex;

    // Start is called before the first frame update
    void Start()
    {

        fogTex = spriteSecondary.secondaryTex;
        currentSprite = currentFramePicker.currentSprite;

    }

    // Update is called once per frame
    void Update()
    {

                if (fogTex == null)
        {
            fogTex = spriteSecondary.secondaryTex;
        }

                
        currentSprite = currentFramePicker.currentSprite;
       // spritePositionOnTexture = currentFramePicker.spritePositionOnTexture;

     

    }

    /*{
        foreach (var material in materials)
            material.SetTexture(fogMaskName, spriteSecondary.secondaryTex);
    }

    {
        foreach (var material in materials)
            material.SetVector(spritePivotName, spritePositionOnTexture);
    }*/

    //.sprite = refSpriteRenderer.sprite.uv;

}

