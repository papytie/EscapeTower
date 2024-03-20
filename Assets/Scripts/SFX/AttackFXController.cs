using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFXController : MonoBehaviour, IAttackFX
{
    Animator animator;

    public void InitFX()
    {
        animator = GetComponent<Animator>();

    }

    public void StartFX()
    {
        animator.SetTrigger(GameParams.Animation.ENEMY_ATTACKFX_TRIGGER);

    }
}
