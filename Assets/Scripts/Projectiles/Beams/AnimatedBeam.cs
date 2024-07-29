using UnityEngine;

public class AnimatedBeam : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    SpriteRenderer spriteRenderer;
    public Animator Animator => animator;
    Animator animator;

    public void Init()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
        animator = GetComponentInChildren<Animator> ();
    }

}
