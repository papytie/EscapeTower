using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyAttack : MonoBehaviour
{
    public int BaseDamage => baseDamage;

    public bool AttackAvailable => !isAttacking && !isOnCooldown;

    [Header("Attack Settings")]
    [SerializeField] int baseDamage = 1;
    [SerializeField] float attackCd = .5f;
    [SerializeField] float attackLag = .2f;
    [SerializeField] float attackRange = .5f;

    [Header("Hitbox Settings")]
    [SerializeField] float hitboxRadius = .5f;
    [SerializeField] float hitboxRange = .5f;
    [SerializeField] float hitboxDuration = .1f;
    [SerializeField] LayerMask playerLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.red;

    Animator animator;
    GameObject target;

    float lagtime = 0;
    float cdTime = 0;
    float durationTime = 0;

    bool isAttacking = false;
    bool isOnCooldown = false;
    bool isTrigger = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void SetTarget(GameObject objectToTarget)
    {
        target = objectToTarget;
    }

    private void Update()
    {
        if (isAttacking)
            LagTimer();

        if (isOnCooldown)
            CooldownTimer();

        if (isTrigger)
            HitboxDetectionTimer();
    }

    void LagTimer()
    {
        lagtime += Time.deltaTime;
        if (lagtime >= attackLag)
        {
            lagtime = 0;
            isAttacking = false;
            animator.SetBool(GameParams.Animation.ENEMY_ATTACKING_BOOL, false);
        }
    }

    void CooldownTimer()
    {
        cdTime += Time.deltaTime;
        if (cdTime >= attackCd)
        {
            cdTime = 0;
            isOnCooldown = false;
        }
    }

    void HitboxDetectionTimer()
    {
        durationTime += Time.deltaTime;
        HitboxDetection();
        if (durationTime >= hitboxDuration)
        {
            durationTime = 0;
            isTrigger = false;
        }
    }

    public void EnemyAttackActivation()
    {
        if (!target) return;

        float dist = Vector3.Distance(target.transform.position, transform.position);
        if (dist <= attackRange)
        {
            Debug.Log("attack launched");
            isAttacking = true;
            isOnCooldown = true;
            isTrigger = true;
            AttackFX();
        }
    }

    void AttackFX()
    {
        animator.SetBool(GameParams.Animation.ENEMY_ATTACKING_BOOL, true);

    }

    public void HitboxDetection()
    {
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, hitboxRadius, targetDirection, hitboxRange, playerLayer);

        if (hit)
            hit.transform.GetComponent<PlayerLifeSystem>().TakeDamage(baseDamage);

    }

    public bool EnemyHitBoxResult(Vector2 origin, Vector2 direction, out RaycastHit2D hit)
    {
        hit = Physics2D.CircleCast(origin, hitboxRadius, direction, hitboxRange, playerLayer);
        return hit ? true : false;
    }

    private void OnDrawGizmos()
    {
        if (showDebug && Application.isPlaying && isTrigger)
        {
            Gizmos.color = colliderDebugColor;
            Vector2 targetDirection = (target.transform.position - transform.position).normalized;
            Gizmos.DrawWireSphere(transform.position.ToVector2() + targetDirection * hitboxRange, hitboxRadius);
            Gizmos.color = Color.white;

        }
    }


}
