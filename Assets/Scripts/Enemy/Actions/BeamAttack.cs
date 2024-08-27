using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime && Vector3.Distance(transform.position, controller.CurrentTargetPos) <= data.activationRange;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    EnemyController controller;
    BeamData data;

    float startTime = 0;
    float cooldownEndTime = 0;
    Vector2 direction = Vector2.zero;
    Quaternion currentRotation = Quaternion.identity;

    AnimatedBeam beam;

    public void Init(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as BeamData;
        controller = controllerRef;

        if (data.beamVisual == null)
        {
            Debug.LogWarning("Beam Visual is missing!");
            return;
        }

        beam = Instantiate(data.beamVisual, transform.position, Quaternion.identity, transform);
        beam.Init();
        beam.gameObject.SetActive(false);
    }

    public void StartProcess()
    {
        direction = (controller.CurrentTargetPos - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
        currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
        beam.gameObject.SetActive(true);
        beam.transform.rotation = currentRotation;
        startTime = Time.time;
    }

    public void UpdateProcess()
    {
        if (Time.time > startTime + data.aimingDuration + data.fireDuration)
            IsCompleted = true;

        Vector3 center = transform.position.ToVector2() + controller.CircleCollider.offset;
        Vector3 beamSpawnPos = center + currentRotation * data.spawnOffset;

        if (Time.time < startTime + data.aimingDuration)
        {
            direction = (controller.CurrentTargetPos - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
            float beamThickness = Mathf.Lerp(0, .25f, (Time.time - startTime) / data.aimingDuration);
            beam.SpriteRenderer.size = new Vector2(GetBeamLength(beamSpawnPos, direction), beamThickness);
            controller.AnimationParam.UpdateMoveAnimDirection(direction);
            currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
            beam.transform.SetPositionAndRotation(beamSpawnPos, currentRotation);
        }

        if (Time.time > startTime + data.aimingDuration + data.ignitionTime)
        {
            beam.Animator.SetBool(SRAnimators.BeamAnim.Parameters.isFiring, true);
            beam.SpriteRenderer.size = new Vector2(GetBeamLength(beamSpawnPos, direction), .5f);
            BeamHit(beamSpawnPos, direction);
        }
    }

    float GetBeamLength(Vector2 start, Vector2 direction)
    {
        RaycastHit2D[] allHit = Physics2D.RaycastAll(start, direction, data.effectiveRange);

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
        return data.effectiveRange;
    }

    void BeamHit(Vector2 start, Vector2 direction)
    {
        RaycastHit2D[] allHit = Physics2D.RaycastAll(start, direction, data.effectiveRange);
        if (allHit.Length > 0)
        {
            foreach (RaycastHit2D hit in allHit)
            {
                if (hit.collider.gameObject.TryGetComponent(out PlayerLifeSystem player))
                {
                    if(!player.IsInvincible)
                        player.TakeDamage(data.baseDamage, Vector2.zero);
                }
            }
        } 
    }

    public void EndProcess()
    {
        beam.gameObject.SetActive(false);
        beam.Animator.SetBool(SRAnimators.BeamAnim.Parameters.isFiring, false);
        cooldownEndTime = Time.time + data.cooldown;
        IsCompleted = false;
    }

}
