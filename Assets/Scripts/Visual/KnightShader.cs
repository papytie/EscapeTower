using UnityEngine;

public class KnightShader : MonoBehaviour
{
    // private Material mat;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color knightColor;

    MaterialPropertyBlock matPropBlock;

    private void Awake()
    {
        InitShader();
        
    }

    void InitShader()
    {
        matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor("_KnightColor", knightColor);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.SetPropertyBlock(matPropBlock);
    }

    private void OnValidate()
    {
        InitShader();
    }
}
