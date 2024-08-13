using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime && Vector3.Distance(transform.position, controller.CurrentTarget.transform.position) <= data.activationRange;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    EnemyController controller;
    BeamData data;

    float startTime = 0;
    float cooldownEndTime = 0;
    Vector2 direction = Vector2.zero;
    Quaternion currentRotation = Quaternion.identity;

    AnimatedBeam beam;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
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
        if (!controller.CurrentTarget) return;

        direction = (controller.CurrentTarget.transform.position.ToVector2() - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
        currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
        beam.gameObject.SetActive(true);
        beam.transform.rotation = currentRotation;
        beam.SpriteRenderer.size = new Vector2(GetBeamLength(direction), beam.SpriteRenderer.size.y);
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
            direction = (controller.CurrentTarget.transform.position.ToVector2() - (controller.CircleCollider.transform.position.ToVector2() + controller.CircleCollider.offset)).normalized;
            beam.SpriteRenderer.size = new Vector2(GetBeamLength(direction), beam.SpriteRenderer.size.y);
            controller.AnimationParam.UpdateMoveAnimDirection(direction);
            currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
            beam.transform.SetLocalPositionAndRotation(beamSpawnPos, currentRotation);
        }

        if (Time.time > startTime + data.aimingDuration)
        {
            beam.Animator.SetBool(SRAnimators.BeamAnim.Parameters.isFiring, true);
            beam.SpriteRenderer.size = new Vector2(GetBeamLength(direction), beam.SpriteRenderer.size.y);
            //Fire Process
        }
    }

    float GetBeamLength(Vector2 direction)
    {
        Vector3 center = transform.position.ToVector2() + controller.CircleCollider.offset;
        Vector3 beamSpawnPos = center + currentRotation * data.spawnOffset;
        RaycastHit2D raycast = Physics2D.Raycast(beamSpawnPos, direction);
        return raycast.distance;
    }

    public void EndProcess()
    {
        beam.gameObject.SetActive(false);
        beam.Animator.SetBool(SRAnimators.BeamAnim.Parameters.isFiring, false);
        cooldownEndTime = Time.time + data.cooldown;
        IsCompleted = false;
    }

}
