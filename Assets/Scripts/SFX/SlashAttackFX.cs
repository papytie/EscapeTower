using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttackFX : MonoBehaviour, IAttackFX
{
    [SerializeField] Animator animatorFX;

    public void InitFX(Animator animator)
    {
        animatorFX = animator;
    }

    public void StartFX()
    {
        animatorFX.SetTrigger(GameParams.Animation.ENEMY_ATTACKFX_TRIGGER);
    }
}
