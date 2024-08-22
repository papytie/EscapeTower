using System;
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
    [SerializeField] bool isInvincible = false;

    float invincibilityEndTime = 0;
    bool isDead = false;

    Animator animator;
    BumpComponent bump;
    Collider2D playerCollider;

    public void InitRef(Animator animatorRef, BumpComponent bumpRef, Collider2D collider)
    {
        animator = animatorRef;
        bump = bumpRef;
        playerCollider = collider;
    }

    void Update()
    {
        if (isInvincible && Time.time >= invincibilityEndTime)
        {
            isInvincible = false;
            playerCollider.enabled = true;
            animator.SetBool(GameParams.Animation.PLAYER_INVINCIBILITY_BOOL, false);
        }
    }

    public void TakeDamage(float damageValue, Vector2 normal)
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
        bump.BumpedAwayActivation(-normal, damageValue);

        animator.SetTrigger(GameParams.Animation.PLAYER_TAKEDAMAGE_TRIGGER);
    }

    public void HealUp(float healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

    public void StartInvincibility(float duration)
    {
        isInvincible = true;
        playerCollider.enabled = false;
        invincibilityEndTime = MathF.Max(invincibilityEndTime, Time.time + duration);
        animator.SetBool(GameParams.Animation.PLAYER_INVINCIBILITY_BOOL, true);
    }

}
