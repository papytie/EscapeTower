using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]

public class EnemyEffectSystem : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void AttackFX()
    {
        animator.SetTrigger(GameParams.Animation.ENEMY_ATTACKFX_TRIGGER);

    }
}
