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
     
       matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor("_VeronColor", eyeColor);
        GetComponent<SpriteRenderer>().SetPropertyBlock(matPropBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
