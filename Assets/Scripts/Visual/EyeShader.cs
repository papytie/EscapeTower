using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeShader : MonoBehaviour
{
    [SerializeField] Color eyeColor;

    SpriteRenderer spriteRenderer;
    MaterialPropertyBlock matPropBlock;

    private void Awake()
    {
        InitShader();

    }

    void InitShader()
    {
        matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor("_EyeColor", eyeColor);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.SetPropertyBlock(matPropBlock);
    }

    private void OnValidate()
    {
        InitShader();
    }
}
