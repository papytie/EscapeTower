using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int BaseDamage => baseDamage;
    public bool IsOnCooldown => isOnCooldown;
    public bool IsAttacking => isAttacking;

    [Header("Attack Settings")]
    [SerializeField] int baseDamage = 1;
    [SerializeField] float attackActivationRange = .5f;
    [Header("Delay")]
    [SerializeField] float attackDelayDuration = .2f;
    [Header("Lag")]
    [SerializeField] float attackLagDuration = .2f;
    [Header("CoolDown")]
    [SerializeField] float attackCooldown = .5f;

    [Header("Hitbox Settings")]
    [SerializeField] float hitboxRadius = .5f;
    [SerializeField] float hitboxRange = .5f;
    [Header("Layer")]
    [SerializeField] LayerMask playerLayer;
    [Header("Duration")]
    [SerializeField] float hitboxDuration = .1f;
    [Header("Delay")]
    [SerializeField] float hitboxDelayDuration = .1f;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.red;

    Animator animator;
    EnemyEffectSystem effects;

    Action OnAttackLagElapsed;
    Action OnAttackCooldownElapsed;
    Action OnAttackDelayElapsed;
    Action OnHitboxDelayElapsed;
    Action OnHitboxDurationElapsed;

    float LagTime = 0;
    float CDTime = 0;
    float durationTime = 0;
    float hitboxDelayTime = 0;
    float attackDelayTime = 0;

    bool isAttacking = false;
    bool isAttackDelayed = false;
    bool isOnCooldown = false;
    bool isHitboxTrigger = false;
    bool isHitboxDelayed = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        //Attack Sequence------------------

        //Initial delay before attacking
        OnAttackDelayElapsed += () =>
        {
            isAttackDelayed = false;
            isHitboxDelayed = true;
            animator.SetTrigger(GameParams.Animation.ENEMY_ATTACK_TRIGGER);
            effects.AttackFX();
        };

        //Delay Hitbox check
        OnHitboxDelayElapsed += () =>
        {
            isHitboxDelayed = false;
            isHitboxTrigger = true;
        };

        //Hitbox check duration
        OnHitboxDurationElapsed += () => 
        { 
            isHitboxTrigger = false; 
        };

        //End attack sequence
        OnAttackLagElapsed += () =>
        {
            isAttacking = false;
            isOnCooldown = true;
        };

        //Attack available again
        OnAttackCooldownElapsed += () =>
        {
            isOnCooldown = false;
        };

    }

    public void InitRef(EnemyEffectSystem effectSystem)
    {
        effects = effectSystem;
    }

    private void Update()
    {

        if (isAttackDelayed)
            CustomTimer(ref attackDelayTime, attackDelayDuration, OnAttackDelayElapsed);

        if (isAttacking)
            CustomTimer(ref LagTime, attackLagDuration, OnAttackLagElapsed);

        if (isHitboxDelayed)
            CustomTimer(ref hitboxDelayTime, hitboxDelayDuration, OnHitboxDelayElapsed);

        if (isHitboxTrigger)
        {
            CustomTimer(ref durationTime, hitboxDuration, OnHitboxDurationElapsed);
            HitboxDetection();
            Debug.Log("hitboxDetection");
        }

        if (isOnCooldown)
            CustomTimer(ref CDTime, attackCooldown, OnAttackCooldownElapsed);
    }

    void CustomTimer(ref float currentTime, float maxTime, Action OnTimeElapsed)
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime)
        {
            currentTime = 0;
            OnTimeElapsed.Invoke();
        }
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
        float dist = Vector3.Distance(target.transform.position, transform.position);
        if (dist <= attackActivationRange)
        {
            isAttackDelayed = true;
            isAttacking = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebug && Application.isPlaying && isHitboxTrigger)
        {
            Gizmos.color = colliderDebugColor;
            Gizmos.DrawSphere(transform.position.ToVector2() + transform.up.ToVector2() * hitboxRange, hitboxRadius);
            Gizmos.color = Color.white;

        }
    }

}
