using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSetter : MonoBehaviour
{

    public SpriteSecondaryTextureReflection secondaryScript;
    public CurrentFramePicker frameScript;
    public SpriteRenderer refSpriteRenderer;

    private SpriteRenderer mySpriteRenderer;
    private Texture2D refSecondaryTex;
    private Rect refSpriteRect;
    private Vector2 refPivot;
    private Texture2D mySecondaryTex;
    private Rect mySpriteRect;
    private Vector2 myPivot;
    private Sprite myCurrentSprite;


    // Start is called before the first frame update
    void Start()
    {
        refSecondaryTex = secondaryScript.secondaryTex;
        refSpriteRect = frameScript.spriteRect;
        refPivot = frameScript.spritePivot;

        mySecondaryTex = refSecondaryTex;
        mySpriteRect = refSpriteRect;
        myPivot = refPivot;
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myCurrentSprite = Sprite.Create(mySecondaryTex, refSpriteRect, myPivot, 100f);
    }

    // Update is called once per frame
    void Update()
    {

        refSecondaryTex = secondaryScript.secondaryTex;

        if (mySecondaryTex != refSecondaryTex)
        {
            mySecondaryTex = refSecondaryTex;
        }

        refSpriteRect = frameScript.spriteRect;

        if (mySpriteRect != refSpriteRect)
        {
            mySpriteRect = refSpriteRect;
        }

        refPivot = frameScript.spritePivot;

        if(myPivot != refPivot)
        {
            myPivot = refPivot;
        }

        myCurrentSprite = Sprite.Create(mySecondaryTex, refSpriteRect, myPivot, 100f);
    }
}
