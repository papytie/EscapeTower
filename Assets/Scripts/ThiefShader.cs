using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefShader : MonoBehaviour
{

   // private Material mat;
    private MaterialPropertyBlock matPropBlock;
    public Color eyeColor;

    // Start is called before the first frame update
    void Start()
    {
      //mat = GetComponent<SpriteRenderer>().material;
       matPropBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        matPropBlock.SetColor("_VeronColor", eyeColor);
        GetComponent<SpriteRenderer>().SetPropertyBlock(matPropBlock);
    }
}
