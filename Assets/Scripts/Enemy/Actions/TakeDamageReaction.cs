using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class TakeDamageReaction : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    EnemyController controller;
    TakeDamageData data;
    float currentTime = 0;
    float startTime = 0;
    float duration = 0;
    float distance = 0;
    Vector2 direction = Vector2.zero;
    Vector2 startPos = Vector2.zero;
    Vector2 targetPos = Vector2.zero;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as TakeDamageData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        startTime = Time.time;

        controller.AnimationParam.ActivateTakeDamageTrigger();

        if (data.activation)
        {
            distance = controller.Stats.LastDMGReceived / 4 / controller.Stats.Weight;
            duration = distance / 4 * controller.Stats.Weight;

            startPos = transform.position;
            targetPos = transform.position + distance * -controller.Stats.LastATKNormalReceived;
            direction = (targetPos - startPos).normalized;

            controller.AnimationParam.UpdateMoveAnimSpeed(distance / duration);
            controller.AnimationParam.UpdateMoveAnimDirection(-direction);
        }
        else
        {
            direction = controller.CurrentDirection;
            controller.AnimationParam.UpdateMoveAnimDirection(direction * .1f);
        }
    }

    public void UpdateProcess()
    {
        float endMovement = startTime + duration;
        float endProcess = startTime + duration + controller.Stats.RecupTime;

        if (Time.time >= endProcess) 
            IsCompleted = true;

        if (data.activation)
        {
            if (Time.time <= endMovement)
            {
                currentTime += Time.deltaTime;

                float t = Mathf.Clamp01(currentTime / duration);

                //Use curve to modify lerp transition
                Vector3 bumpTargetPos = Vector3.Lerp(startPos, targetPos, data.moveCurve.Evaluate(t));

                //Calculate value of next Dash movement
                float bumpStepValue = (bumpTargetPos - transform.position).magnitude;

                //Check at next dash step position if collision occurs
                controller.Collision.MoveToCollisionCheck(direction, bumpStepValue, controller.Collision.BlockingObjectsLayer, out Vector3 fixedPosition, out List<RaycastHit2D> hitList);

                if (hitList.Count > 0)
                {
                    transform.position = fixedPosition;
                    startTime -= duration - currentTime;
                }
                else
                    transform.position = bumpTargetPos;
            }

            if (Time.time > endMovement)
                controller.AnimationParam.UpdateMoveAnimDirection(controller.Stats.LastATKNormalReceived * .1f);
        }

    }

    public void EndProcess()
    {
        currentTime = 0;
        IsCompleted = false;
        controller.CurrentDirection = direction;
    }
}
