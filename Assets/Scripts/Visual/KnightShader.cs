using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightShader : MonoBehaviour
{
    // private Material mat;
    private MaterialPropertyBlock matPropBlock;
    public Color knightColor;

    // Start is called before the first frame update
    void Start()
    {

        matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor("_KnightColor", knightColor);
        GetComponent<SpriteRenderer>().SetPropertyBlock(matPropBlock);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
