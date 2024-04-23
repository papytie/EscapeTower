using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormShader : MonoBehaviour
{
    // private Material mat;
    private MaterialPropertyBlock matPropBlock;
    public Color wormColor;

    // Start is called before the first frame update
    void Start()
    {

        matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor("_WormColor", wormColor);
        GetComponent<SpriteRenderer>().SetPropertyBlock(matPropBlock);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
