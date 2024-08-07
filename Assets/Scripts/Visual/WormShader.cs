using UnityEngine;

public class WormShader : MonoBehaviour
{
    [SerializeField] Color wormColor = Color.green;

    SpriteRenderer spriteRenderer;
    MaterialPropertyBlock matPropBlock;

    void Start()
    {
        InitShader();
    }

    void InitShader()
    {
        matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor("_WormColor", wormColor);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.SetPropertyBlock(matPropBlock);
    }

    private void OnValidate()
    {
        InitShader();
    }
}
