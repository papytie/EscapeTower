using System.Collections.Generic;
using UnityEngine;

public class FleeMove : MonoBehaviour, IAction
{
    public Vector2 MoveDirection { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsAvailable => throw new System.NotImplementedException();

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
        Vector3 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        MoveDirection = (controller.CurrentTarget.transform.position - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, controller.CurrentTarget.transform.position);

        if (targetDistance < data.maxRange)
        {

            //Check for collision
            controller.Collision.MoveToCollisionCheck(-MoveDirection, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
            transform.position = finalPosition;


        }
    }

    public void EndProcess()
    {
        throw new System.NotImplementedException();
    }
}
