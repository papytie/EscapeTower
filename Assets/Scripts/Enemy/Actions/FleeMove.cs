using System.Collections.Generic;
using UnityEngine;

public class FleeMove : MonoBehaviour, IAction
{
    public bool IsCompleted { get; set; }
    public bool IsAvailable => throw new System.NotImplementedException();
    public Vector3 Direction => direction;
    Vector2 direction;

    private FleeData data;
    private EnemyController controller;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as FleeData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
    }

    public void UpdateProcess()
    {
        Vector2 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        direction = (controller.CurrentTargetPos - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, controller.CurrentTargetPos);

        if (targetDistance < data.maxRange)
        {

            //Check for collision
            controller.Collision.MoveToCollisionCheck(-direction, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
            transform.position = finalPosition;


        }
    }

    public void EndProcess()
    {
    }
}
