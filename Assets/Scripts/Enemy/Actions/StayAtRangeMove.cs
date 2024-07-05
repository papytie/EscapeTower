using System.Collections.Generic;
using UnityEngine;

public class StayAtRangeMove : MonoBehaviour, IAction
{
    public bool IsCompleted { get; set; }
    public bool IsAvailable => true;
    public Vector3 Direction => direction;
    Vector2 direction;

    private StayAtRangeData data;
    private EnemyController controller;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as StayAtRangeData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
    }

    public void UpdateProcess()
    {
        if (controller.CurrentTarget == null) return;

        Vector3 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        direction = (controller.CurrentTarget.transform.position - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, controller.CurrentTarget.transform.position);

        if (targetDistance > data.maxRange || targetDistance < data.minRange)
        {
            //Invert direction if too close of target
            if (targetDistance < data.minRange) direction = -direction;

            //Check for collision
            controller.Collision.MoveToCollisionCheck(direction, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
            if (hitList.Count > 0) return;
            transform.position = finalPosition;

            controller.AnimationParam.UpdateMoveAnimDirection(direction);
        }
    }

    public void EndProcess()
    {
        IsCompleted = false;
        controller.CurrentDirection = direction;
    }
}
