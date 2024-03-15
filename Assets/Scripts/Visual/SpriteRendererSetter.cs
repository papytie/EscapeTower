using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererSetter : MonoBehaviour
{

    public SpriteRenderer refSpriteRenderer;
    private SpriteRenderer mySpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (mySpriteRenderer.sprite != refSpriteRenderer.sprite)
        {

            mySpriteRenderer.sprite = refSpriteRenderer.sprite;

        }
    }
}
