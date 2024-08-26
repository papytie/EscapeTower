using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime && Vector3.Distance(transform.position, controller.CurrentTargetPos) <= data.activationRange;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    EnemyController controller;
    RangedData data;

    bool updateWarnings = false;
    float cooldownEndTime = 0;
    float warningStartTime = 0;
    Vector2 direction = Vector2.zero;
    Vector2 targetPos = Vector2.zero;
    Quaternion currentRotation = Quaternion.identity;

    PlayerController playerController = null;
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
        if(data.projectileData.projectileToSpawn == null) 
        {
            Debug.LogWarning("No valid Projectile");
            IsCompleted = true;
            return;
        }

        if (data.anticipatedAiming)
        {
            playerController = controller.CurrentTarget.GetComponent<PlayerController>();
            if (playerController == null) return;

            Vector2 playerNextPosSecond = playerController.MoveInput * playerController.Movement.CurrentSpeed;
            float collisionTime = Vector2.Distance(transform.position, controller.CurrentTargetPos) / data.projectileData.speed;
            targetPos = controller.CurrentTargetPos + playerNextPosSecond * collisionTime;
        }
        else targetPos = controller.CurrentTargetPos;

        direction = (targetPos - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
        currentRotation = Quaternion.LookRotation(Vector3.forward, direction);

        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed);

        StartCoroutine(AttackProcess());
    }

    public void UpdateProcess()
    {
        if (updateWarnings)
        {
            targetPos = controller.CurrentTargetPos;

            if (data.anticipatedAiming && playerController)
            {
                Vector2 playerNextPosSecond = playerController.MoveInput * playerController.Movement.CurrentSpeed;
                float collisionTime = Vector2.Distance(transform.position, controller.CurrentTargetPos) / data.projectileData.speed;
                targetPos = controller.CurrentTargetPos + playerNextPosSecond * collisionTime;
            }

            if (data.progressiveAiming)
            {
                direction = (targetPos - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
                controller.AnimationParam.UpdateMoveAnimDirection(direction);
                currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
            }

            //Try a progressive rotation
            //currentRotation = Quaternion.RotateTowards(currentRotation, Quaternion.LookRotation(Vector3.forward, direction), 10 * Time.deltaTime);

            Vector3 center = transform.position.ToVector2() + controller.CircleCollider.offset;
            Vector3 projectileSpawnPos = center + currentRotation * data.projectileData.spawnOffset;

            float beamThickness = Mathf.Lerp(.05f, data.projectileData.projectileToSpawn.CircleRadius * 2, (Time.time - warningStartTime) / data.warningTime);

            if (data.projectileData.spawnNumber > 1)
            {
                float minAngle = data.projectileData.spreadAngle / 2f;
                float angleIncrValue = data.projectileData.spreadAngle / (data.projectileData.spawnNumber - 1);

                for (int i = 0; i < data.projectileData.spawnNumber; i++)
                {
                    float angle = minAngle - i * angleIncrValue;
                    Quaternion angleResult = Quaternion.AngleAxis(angle + data.projectileData.angleOffset, transform.forward);

                    warningBeams[i].transform.SetPositionAndRotation(projectileSpawnPos, currentRotation * angleResult);
                    warningBeams[i].SpriteRenderer.size = new Vector2(GetBeamLength(angleResult * direction), beamThickness);
                }
            }
            else
            {
                warningBeams[0].transform.SetPositionAndRotation(projectileSpawnPos, currentRotation * Quaternion.AngleAxis(data.projectileData.angleOffset, transform.forward));
                warningBeams[0].SpriteRenderer.size = new Vector2(GetBeamLength(direction), beamThickness);
            }
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
        controller.Animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.attack);

        if(data.displayWarningBeam)
        {
            updateWarnings = true;
            warningStartTime = Time.time;

            if (data.projectileData.spawnNumber > 1)
                for (int i = 0; i < data.projectileData.spawnNumber; i++)
                    warningBeams[i].gameObject.SetActive(true);

            else warningBeams[0].gameObject.SetActive(true);
           
            yield return new WaitForSeconds(data.warningTime);
            updateWarnings = false;
            yield return new WaitForSeconds(data.reactionTime);

            foreach (AnimatedBeam beam in warningBeams)
                beam.gameObject.SetActive(false);     
        }


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
               
        RaycastHit2D[] allHit = Physics2D.RaycastAll(beamSpawnPos, direction, data.projectileData.range);
        if (allHit.Length > 0)
        {
            foreach (RaycastHit2D hit in allHit)
            {
                if ((controller.Detection.ObstructionLayer & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    return hit.distance; 
                }
            }
        }
        
        return data.projectileData.range;
    }
}
