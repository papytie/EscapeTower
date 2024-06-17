using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour, IAttack
{
    public bool IsOnAttackLag => isOnAttackLag;
    public bool IsAttacking => isAttacking;
    public bool IsOnCooldown => isOnCooldown;
    public float AttackRange => rangedAttackData.projectileData.range;

    IAttackFX attackFX;
    EnemyStatsComponent stats;
    RangedAttackData rangedAttackData;
    EnemyDetectionComponent detectionComponent;
    EnemyController enemyController;
    CircleCollider2D circleCollider;
    Animator animator;
    List<PlayerLifeSystem> playerHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    bool isAttacking = false;
    Quaternion currentRotation = Quaternion.identity;

    public void Init(IAttackData dataRef, EnemyStatsComponent statsComponentRef, Animator animatorRef)
    {
        rangedAttackData = dataRef as RangedAttackData;
        //attackFX = attackFXRef;
        stats = statsComponentRef;
        animator = animatorRef;
    }

    private void Update()
    {    
        if (isOnAttackLag && Time.time >= attackLagEndTime)
            isOnAttackLag = false;

        if (isOnCooldown && Time.time >= cooldownEndTime)
            isOnCooldown = false;
    }

    public void AttackActivation()
    {
        isAttacking = true;
        StartAttackLag();
        StartCoroutine(AttackProcess());

        animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.attack);
        //attackFX.StartFX(enemyController.CurrentDirection);

        //For Debug infos
        currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.CurrentDirection);
    }

    IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(rangedAttackData.delay);

        StartCoroutine(FireProjectile());
        StartAttackCooldown();
        isAttacking = false;
    }

    IEnumerator FireProjectile()
    {
        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.CurrentDirection);
        Vector3 center = transform.position.ToVector2() + circleCollider.offset;
        Vector3 projectileSpawnPos = center + currentRotation * rangedAttackData.projectileData.spawnOffset;

        if (rangedAttackData.projectileData.spawnNumber > 1)
        {
            float minAngle = rangedAttackData.projectileData.spreadAngle / 2f;
            float angleIncrValue = rangedAttackData.projectileData.spreadAngle / (rangedAttackData.projectileData.spawnNumber - 1);

            for (int i = 0; i < rangedAttackData.projectileData.spawnNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + rangedAttackData.projectileData.angleOffset, base.transform.forward);

                Instantiate(rangedAttackData.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * angleResult)
                    .Init(enemyController.gameObject, rangedAttackData.projectileData, projectileSpawnPos, stats.ProjectileDamage, rangedAttackData.projectileData.range);

                if (rangedAttackData.projectileData.spawnType == ProjectileSpawnType.Sequence)
                {
                    float t = rangedAttackData.duration / (rangedAttackData.projectileData.spawnNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(rangedAttackData.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * Quaternion.AngleAxis(rangedAttackData.projectileData.angleOffset, base.transform.forward))
                .Init(enemyController.gameObject, rangedAttackData.projectileData, projectileSpawnPos, stats.ProjectileDamage, rangedAttackData.projectileData.range);
    }

    void StartAttackCooldown()
    {
        cooldownEndTime = Time.time + rangedAttackData.cooldown;
        isOnCooldown = true;
    }

    void StartAttackLag()
    {
        attackLagEndTime = Time.time + rangedAttackData.lag;
        isOnAttackLag = true;
    }

    private void OnValidate()
    {
        enemyController = gameObject.GetComponent<EnemyController>();
        circleCollider = gameObject.GetComponent<CircleCollider2D>();
    }

/*    private void OnDrawGizmos()
    {
        if (rangedAttackData.projectileData.showDebug && rangedAttackData != null)
        {
            Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.CurrentDirection);
            Vector3 center = transform.position.ToVector2() + circleCollider.offset;
            Vector3 projectileSpawnPos = center + currentRotation * rangedAttackData.projectileData.spawnOffset;

            Gizmos.color = rangedAttackData.projectileData.projectileDebugColor;
            Gizmos.DrawWireSphere(projectileSpawnPos, .02f);
            if (rangedAttackData.projectileData.spawnNumber > 1)
            {
                float minAngle = rangedAttackData.projectileData.spreadAngle / 2f;
                float angleIncrValue = rangedAttackData.projectileData.spreadAngle / (rangedAttackData.projectileData.spawnNumber - 1);

                for (int i = 0; i < rangedAttackData.projectileData.spawnNumber; i++)
                {
                    float angle = minAngle - i * angleIncrValue;
                    Quaternion angleRotation = Quaternion.AngleAxis(angle + rangedAttackData.projectileData.angleOffset, gameObject.transform.forward);

                    Vector3 multProjPos = projectileSpawnPos + currentRotation * angleRotation * Vector3.up * rangedAttackData.projectileData.range;
                    Vector3 multProjHitboxEndPos = multProjPos + currentRotation * angleRotation * rangedAttackData.projectileData.projectileToSpawn.HitboxOffset;

                    switch (rangedAttackData.projectileData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(multProjHitboxEndPos, rangedAttackData.projectileData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(rangedAttackData.projectileData.debugCube, -1, multProjHitboxEndPos, currentRotation * angleRotation, rangedAttackData.projectileData.projectileToSpawn.BoxSize);
                            break;
                    }
                    Gizmos.DrawLine(projectileSpawnPos, multProjPos);
                }
            }
            else
            {
                Vector3 singleProjEndPos = projectileSpawnPos + currentRotation * Vector3.up * rangedAttackData.projectileData.range;

                switch (rangedAttackData.projectileData.projectileToSpawn.HitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(singleProjEndPos, rangedAttackData.projectileData.projectileToSpawn.CircleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(rangedAttackData.projectileData.debugCube, -1, singleProjEndPos, gameObject.transform.rotation * Quaternion.AngleAxis(rangedAttackData.projectileData.angleOffset, transform.forward), rangedAttackData.projectileData.projectileToSpawn.BoxSize);
                        break;
                }
                Gizmos.DrawLine(projectileSpawnPos, singleProjEndPos);
            }
        }
    }
*/}
