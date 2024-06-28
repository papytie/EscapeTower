using System.Collections.Generic;
using UnityEngine;

public class ChaseMove : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted { get; set; }

    private ChaseData data;
    private EnemyController controller;
    Vector2 direction;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as ChaseData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed * data.speedMult);
    }

    public void UpdateProcess()
    {
        if (controller.CurrentTarget == null) return;

        Vector3 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        direction = (controller.CurrentTarget.transform.position - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, controller.CurrentTarget.transform.position);

        if (targetDistance > data.minRange)
        {
            //Check for collision
            controller.Collision.MoveToCollisionCheck(direction, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
            transform.position = finalPosition;

            //Update Animation
            controller.AnimationParam.UpdateMoveAnimDirection(direction);
        }

    }

    public void EndProcess()
    {
    }
}
