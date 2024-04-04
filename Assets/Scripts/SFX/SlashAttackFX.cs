using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlashAttackFX : MonoBehaviour, IAttackFX
{
    [SerializeField] Animator animatorFX;

    public void StartFX(Vector2 direction)
    {
        animatorFX.SetTrigger(GameParams.Animation.ENEMY_ATTACKFX_TRIGGER);
    }
}
