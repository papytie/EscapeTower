using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeSystem : MonoBehaviour, ILifeSystem
{
    public bool IsDead => isDead;
    public bool IsInvincible { get => isInvincible; set { isInvincible = value; } }
    public float CurrentLifePoints => currentLifePoints;
    public float MaxLifePoints => maxLifePoints;

    [SerializeField] float currentLifePoints = 10;
    [SerializeField] float maxLifePoints = 20;
    [SerializeField] float InvincibilityDuration = .2f;

    float invincibilityEndTime = 0;
    bool isDead = false;
    bool isInvincible = false;

    Animator animator;

    public void InitRef(Animator animatorRef)
    {
        animator = animatorRef;
    }

    void Update()
    {
        if (isInvincible && Time.time >= invincibilityEndTime)
            isInvincible = false;
    }

    public void TakeDamage(float damageValue)
    {
        if (isDead || isInvincible) return;

        StartInvincibility(InvincibilityDuration);
        currentLifePoints -= damageValue;
        if (currentLifePoints <= 0) 
        {
            currentLifePoints = 0;
            isDead = true;
            animator.SetTrigger(GameParams.Animation.PLAYER_DIE_TRIGGER);
            return;
        }
        animator.SetTrigger(GameParams.Animation.PLAYER_TAKEDAMAGE_TRIGGER);
    }

    public void HealUp(float healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

    public void StartInvincibility(float duration)
    {
        invincibilityEndTime = MathF.Max(invincibilityEndTime, Time.time + duration);
        isInvincible = true;
    }

}
