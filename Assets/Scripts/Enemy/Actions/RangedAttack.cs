using System.Collections;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime && Vector3.Distance(transform.position, controller.CurrentTarget.transform.position) <= controller.Stats.ProjectileRange;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    EnemyController controller;
    RangedData data;

    float processEndTime = 0;
    float cooldownEndTime = 0;
    Vector2 direction = Vector2.zero;
    Quaternion currentRotation = Quaternion.identity;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as RangedData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        IsCompleted = false;
        processEndTime = Time.time + controller.Stats.ReactionTime + data.duration;

        //attackFX.StartFX(enemyController.CurrentDirection);

        if (!controller.CurrentTarget) return;

        direction = (controller.CurrentTarget.transform.position.ToVector2() - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
        currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed);
        StartCoroutine(AttackProcess());
    }

    public void UpdateProcess()
    {
        if (Time.time >= processEndTime)
            IsCompleted = true;
    }

    public void EndProcess()
    {
        cooldownEndTime = Time.time + controller.Stats.ProjectileCooldown;
    }

    IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(controller.Stats.ReactionTime);
        controller.Animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.attack);
        StartCoroutine(FireProjectile());
    }

    IEnumerator FireProjectile()
    {
        Vector3 center = transform.position.ToVector2() + controller.CircleCollider.offset;
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
                    float t = data.duration / (data.projectileData.spawnNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(data.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * Quaternion.AngleAxis(data.projectileData.angleOffset, transform.forward))
                .Init(controller.gameObject, data.projectileData, projectileSpawnPos, controller.Stats.ProjectileDamage, data.projectileData.range);
    }
}
