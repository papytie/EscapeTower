using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime;
    public bool IsCompleted { get; set; }

    EnemyController controller;
    CircleCollider2D circleCollider;
    RangedData data;

    float processEndTime = 0;
    float cooldownEndTime = 0;
    Quaternion currentRotation = Quaternion.identity;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as RangedData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        IsCompleted = false;
        processEndTime = Time.time + data.delay + data.attackDuration;


        //attackFX.StartFX(enemyController.CurrentDirection);

        if (!controller.TargetAcquired) return;

        Vector3 targetDirection = (controller.CurrentTarget.transform.position.ToVector2() - (circleCollider.transform.position.ToVector2() + circleCollider.offset)).normalized;
        currentRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
        controller.AnimationParam.UpdateMoveAnimDirection(targetDirection);
        StartCoroutine(AttackProcess());
    }

    public void UpdateProcess()
    {
        if (Time.time >= processEndTime)
            IsCompleted = true;
    }

    public void EndProcess()
    {
        cooldownEndTime = Time.time + data.cooldown;
    }

    IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(data.delay);
        StartCoroutine(FireProjectile());
        controller.Animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.attack);
    }

    IEnumerator FireProjectile()
    {
        Vector3 center = transform.position.ToVector2() + circleCollider.offset;
        Vector3 projectileSpawnPos = center + currentRotation * data.projectileData.spawnOffset;

        if (data.projectileData.spawnNumber > 1)
        {
            float minAngle = data.projectileData.spreadAngle / 2f;
            float angleIncrValue = data.projectileData.spreadAngle / (data.projectileData.spawnNumber - 1);

            for (int i = 0; i < data.projectileData.spawnNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + data.projectileData.angleOffset, transform.forward);

                Instantiate(data.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * angleResult)
                    .Init(controller.gameObject, data.projectileData, projectileSpawnPos, controller.Stats.ProjectileDamage, data.projectileData.range);

                if (data.projectileData.spawnType == ProjectileSpawnType.Sequence)
                {
                    float t = data.attackDuration / (data.projectileData.spawnNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(data.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * Quaternion.AngleAxis(data.projectileData.angleOffset, transform.forward))
                .Init(controller.gameObject, data.projectileData, projectileSpawnPos, controller.Stats.ProjectileDamage, data.projectileData.range);
    }

    private void OnValidate()
    {
        controller = gameObject.GetComponent<EnemyController>();
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
    */
}
