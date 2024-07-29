using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class BeamFireAnimated : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    SpriteRenderer spriteRenderer;

    public void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

}
