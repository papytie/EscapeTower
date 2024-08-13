using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime && Vector3.Distance(transform.position, controller.CurrentTarget.transform.position) <= data.activationRange;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    private EnemyController controller;
    private ChargeData data;

    float currentTime = 0;
    float cooldownEndTime = 0;
    float startTime = 0;
    float duration = 0;
    float distance = 0;

    Vector2 direction = Vector2.zero;
    Vector2 startPos = Vector2.zero;
    Vector2 targetPos = Vector2.zero;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as ChargeData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        startTime = Time.time;

        distance = data.effectiveRange;
        duration = distance / 5 * controller.Stats.Weight;

        startPos = transform.position;
        targetPos = startPos + (controller.CurrentTarget.transform.position.ToVector2() - startPos).normalized * distance;

        direction = (targetPos - startPos).normalized;

        controller.AnimationParam.UpdateMoveAnimSpeed(distance / duration);
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
    }

    public void UpdateProcess()
    {
        float startMovement = startTime + data.reactionTime;
        float endMovement = startTime + data.reactionTime + duration;
        float endProcess = startTime + data.reactionTime + duration + data.recupTime;

        if (Time.time >= endProcess)
            IsCompleted = true;

        if(Time.time >= startMovement && Time.time <= endMovement)
        {
            currentTime += Time.deltaTime;

            float t = Mathf.Clamp01(currentTime / duration);

            //Use curve to modify lerp transition
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, data.moveCurve.Evaluate(t));

            //Calculate value of next Dash movement
            float stepValue = (currentPos - transform.position).magnitude;

            //Check at next dash step position if collision occurs
            controller.Collision.MoveToCollisionCheck(direction, stepValue, data.obstructionLayer, out Vector3 fixedPosition, out List<RaycastHit2D> hitList);

            if (hitList.Count > 0)
            {
                transform.position = fixedPosition;
                startTime -= duration - currentTime;
            }
            else
                transform.position = currentPos;
        }

        if (Time.time > endMovement)
            controller.AnimationParam.UpdateMoveAnimDirection(controller.Stats.LastATKNormalReceived * .1f);
    }

    public void EndProcess()
    {
        IsCompleted = false;
        currentTime = 0;
        cooldownEndTime = Time.time + data.cooldown;
        controller.CurrentDirection = direction;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && data.showDebug && controller.MainBehaviour.FSM.CurrentState.Action.GetType() == typeof(ChargeAttack))
        {
            Gizmos.color = data.startColor;
            Gizmos.DrawWireSphere(startPos, controller.CircleCollider.radius);
            Gizmos.color = data.targetColor;
            Gizmos.DrawWireSphere(targetPos, controller.CircleCollider.radius);
            Gizmos.DrawSphere(transform.position, controller.CircleCollider.radius);
            Gizmos.DrawLine(startPos, targetPos);
        }
    }
}
