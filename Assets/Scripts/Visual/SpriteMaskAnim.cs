using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMaskAnim : MonoBehaviour
{

    private SpriteMask spriteMask;
    public SpriteRenderer refSpriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        spriteMask = GetComponent<SpriteMask>();
    }


    private void LateUpdate()
    {

        if (spriteMask.sprite != refSpriteRenderer.sprite)
        {

            spriteMask.sprite = refSpriteRenderer.sprite;

        }
    }
}
