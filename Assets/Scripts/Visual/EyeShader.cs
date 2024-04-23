using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeShader : MonoBehaviour
{
    // private Material mat;
    private MaterialPropertyBlock matPropBlock;
    public Color eyeColor;

    // Start is called before the first frame update
    void Start()
    {

        matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor("_EyeColor", eyeColor);
        GetComponent<SpriteRenderer>().SetPropertyBlock(matPropBlock);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
