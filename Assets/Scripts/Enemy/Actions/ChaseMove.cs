using System.Collections.Generic;
using UnityEngine;

public class ChaseMove : MonoBehaviour, IAction
{
    public bool IsAvailable => Vector2.Distance(controller.CurrentTargetPos, transform.position) > data.minRange;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    private ChaseData data;
    private EnemyController controller;
    Vector2 direction;

    public void Init(IActionData dataRef, EnemyController controllerRef)
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
        Vector2 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        direction = (controller.CurrentTargetPos - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, controller.CurrentTargetPos);

        if (targetDistance > data.minRange)
        {
            //Check for collision
            controller.Collision.MoveToCollisionCheck(direction, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);

            //The right movement direction vector modified by the collision Check
            Vector2 moveVector = (finalPosition - transform.position).normalized;

            //Actual movement
            transform.position = finalPosition;

            //Update Animation with the right direction vector
            controller.AnimationParam.UpdateMoveAnimDirection(moveVector);

            if(data.dropConfig.item != null)
            {
                controller.DropComponent.DropItem(data.dropConfig);
            }
        }
        else IsCompleted = true;
    }

    public void EndProcess()
    {
        IsCompleted = false;
        controller.CurrentDirection = direction;
    }
}
