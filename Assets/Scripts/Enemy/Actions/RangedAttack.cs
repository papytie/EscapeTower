using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime && Vector3.Distance(transform.position, controller.CurrentTarget.transform.position) <= data.activationRange;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    EnemyController controller;
    RangedData data;

    float cooldownEndTime = 0;
    Vector2 direction = Vector2.zero;
    Quaternion currentRotation = Quaternion.identity;

    List<AnimatedBeam> warningBeams = new(); 

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as RangedData;
        controller = controllerRef;

        if(data.displayWarningBeam)
        {
            for (int i = 0; i < data.projectileData.spawnNumber; i++)
            {
                AnimatedBeam beam = Instantiate(data.warningBeam, transform.position, Quaternion.identity, transform);
                beam.Init();
                warningBeams.Add(beam);            
                beam.gameObject.SetActive(false);
            }
        }
    }

    public void StartProcess()
    {
        if(data.projectileData.projectileToSpawn == null || !controller.CurrentTarget) 
        {
            Debug.LogWarning("No valid Projectile");
            IsCompleted = true;
            return;
        }

        direction = (controller.CurrentTarget.transform.position.ToVector2() - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
        currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed);

        StartCoroutine(AttackProcess());
    }

    public void UpdateProcess()
    {
        if (!controller.TargetAcquired)
        {
            if (data.displayWarningBeam)
                foreach (AnimatedBeam beam in warningBeams)
                    beam.gameObject.SetActive(false);

            IsCompleted = true;
        }
    }

    public void EndProcess()
    {
        StopCoroutine(AttackProcess());
        cooldownEndTime = Time.time + data.cooldown;
        IsCompleted = false;
    }

    IEnumerator AttackProcess()
    {
        Vector3 center = transform.position.ToVector2() + controller.CircleCollider.offset;
        Vector3 projectileSpawnPos = center + currentRotation * data.projectileData.spawnOffset;

        yield return new WaitForSeconds(data.reactionTime);
        
        controller.Animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.attack);

        ///////////////////////DISPLAY WARNING BEAM//////////////////////
        if(data.displayWarningBeam)
        {
            if (data.projectileData.spawnNumber > 1)
            {
                float minAngle = data.projectileData.spreadAngle / 2f;
                float angleIncrValue = data.projectileData.spreadAngle / (data.projectileData.spawnNumber - 1);

                for (int i = 0; i < data.projectileData.spawnNumber; i++)
                {
                    float angle = minAngle - i * angleIncrValue;
                    Quaternion angleResult = Quaternion.AngleAxis(angle + data.projectileData.angleOffset, transform.forward);

                    warningBeams[i].transform.SetPositionAndRotation(projectileSpawnPos, currentRotation * angleResult);
                    warningBeams[i].SpriteRenderer.size = new Vector2(GetBeamLength(angleResult * direction), data.projectileData.projectileToSpawn.CircleRadius * 2);
                    warningBeams[i].gameObject.SetActive(true);
                }
            }
            else
            {
                warningBeams[0].transform.SetPositionAndRotation(projectileSpawnPos, currentRotation * Quaternion.AngleAxis(data.projectileData.angleOffset, transform.forward));
                warningBeams[0].SpriteRenderer.size = new Vector2(GetBeamLength(direction), data.projectileData.projectileToSpawn.CircleRadius * 2);
                warningBeams[0].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(data.warningTime);

            foreach (AnimatedBeam beam in warningBeams)
            {
                beam.gameObject.SetActive(false);
            }
        }
        ///////////////////////DISPLAY WARNING BEAM//////////////////////
        
        if (data.projectileData.spawnNumber > 1)
        {
            float minAngle = data.projectileData.spreadAngle / 2f;
            float angleIncrValue = data.projectileData.spreadAngle / (data.projectileData.spawnNumber - 1);

            for (int i = 0; i < data.projectileData.spawnNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + data.projectileData.angleOffset, transform.forward);

                Instantiate(data.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * angleResult)
                    .Init(controller.gameObject, data.projectileData, projectileSpawnPos, data.baseDamage * controller.Stats.ScalingFactor, data.projectileData.range);

                if (data.projectileData.spawnType == ProjectileSpawnType.Sequence)
                {
                    float t = data.duration / (data.projectileData.spawnNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(data.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * Quaternion.AngleAxis(data.projectileData.angleOffset, transform.forward))
                .Init(controller.gameObject, data.projectileData, projectileSpawnPos, data.baseDamage * controller.Stats.ScalingFactor, data.projectileData.range);

        yield return new WaitForSeconds(data.recupTime);

        IsCompleted = true;
    }

    float GetBeamLength(Vector2 direction)
    {
        Vector3 center = transform.position.ToVector2() + controller.CircleCollider.offset;
        Vector3 beamSpawnPos = center + currentRotation * data.projectileData.spawnOffset;
               
        RaycastHit2D[] allHit = Physics2D.RaycastAll(beamSpawnPos, direction);
        if (allHit.Length < 0)
        {
            foreach (RaycastHit2D hit in allHit)
            {
                if (hit.transform.gameObject.layer == data.projectileData.obstructionLayer)
                    if (hit.distance < data.projectileData.range)
                        return hit.distance;     
            }
        }
        return data.projectileData.range;
    }
}
