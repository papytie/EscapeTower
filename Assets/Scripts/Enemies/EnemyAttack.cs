using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int BaseDamage => baseDamage;
    public bool IsOnCooldown => isOnCooldown;
    public bool IsAttacking => isOnAttackLag;

    [Header("Attack Settings")]
    [SerializeField] int baseDamage = 1;
    [SerializeField] float attackActivationRange = .5f;
    [SerializeField] float attackDelayDuration = .2f;
    [SerializeField] float attackLagDuration = .2f;
    [SerializeField] float attackCooldownDuration = .5f;

    [Header("Hitbox Settings")]
    [SerializeField] float hitboxRadius = .5f;
    [SerializeField] float hitboxRange = .5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float hitboxDetectionDuration = .1f;
    [SerializeField] float hitboxDelayDuration = .1f;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.red;

    Animator animator;
    EnemyEffectSystem effects;

    float attackLagEndTime = 0;
    float attackCoolDownEndTime = 0;
    float hitboxDetectionEndTime = 0;
    float hitboxDelayEndTime = 0;
    float attackDelayEndTime = 0;

    bool isOnAttackLag = false;
    bool isAttackDelayed = false;
    bool isOnCooldown = false;
    bool isOnHitboxDetection = false;
    bool isHitboxDelayed = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void InitRef(EnemyEffectSystem effectSystem)
    {
        effects = effectSystem;
    }

    private void Update()
    {
        if (isAttackDelayed && Time.time >= attackDelayEndTime)
        {
            isAttackDelayed = false;
            SetHitboxDelayTimer(hitboxDelayDuration);
            animator.SetTrigger(GameParams.Animation.ENEMY_ATTACK_TRIGGER);
            effects.AttackFX();
        }

        if (isOnAttackLag && Time.time >= attackLagEndTime)
        {
            isOnAttackLag = false;
            SetAttackCooldownTimer(attackCooldownDuration);
        }

        if (isHitboxDelayed && Time.time >= hitboxDelayEndTime)
        {
            isHitboxDelayed = false;
            SetHitboxDetectionTimer(hitboxDetectionDuration);
        }

        if (isOnHitboxDetection)
        {
            HitboxDetection();
            if (Time.time >= hitboxDetectionEndTime)
                isOnHitboxDetection = false;
        }

        if (isOnCooldown && Time.time >= attackCoolDownEndTime)
            isOnCooldown = false;

    }


    void HitboxDetection()
    {
        //Check for player presence then apply damage
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, hitboxRadius, transform.up, hitboxRange, playerLayer);
        if (hit)
            hit.transform.GetComponent<PlayerLifeSystem>().TakeDamage(baseDamage);
    }

    public void EnemyAttackActivation(GameObject target)
    {
        if (!target || isOnCooldown) return;

        //Check range from target and launch attack sequence
        Vector2 toTargetVector = target.transform.position - transform.position;
        if (toTargetVector.magnitude <= attackActivationRange)
        {
            //Look At Rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, toTargetVector.normalized);

            SetAttackDelayTimer(attackDelayDuration);
            SetAttackLagTimer(attackLagDuration);
        }
    }

    void SetAttackDelayTimer(float duration)
    {
        attackDelayEndTime = Time.time + duration;
        isAttackDelayed = true;
    }

    void SetHitboxDelayTimer(float duration)
    {
        hitboxDelayEndTime = Time.time + duration;
        isHitboxDelayed = true;
    }

    void SetHitboxDetectionTimer(float duration)
    {
        hitboxDetectionEndTime = Time.time + duration;
        isOnHitboxDetection = true;
    }

    void SetAttackCooldownTimer(float duration)
    {
        attackCoolDownEndTime = Time.time + duration;
        isOnCooldown = true;
    }

    void SetAttackLagTimer(float duration)
    {
        attackLagEndTime = Time.time + duration;
        isOnAttackLag = true;
    }

    private void OnDrawGizmos()
    {
        if (showDebug && Application.isPlaying && isOnHitboxDetection)
        {
            Gizmos.color = colliderDebugColor;
            Gizmos.DrawSphere(transform.position.ToVector2() + transform.up.ToVector2() * hitboxRange, hitboxRadius);
            Gizmos.color = Color.white;

        }
    }

}
