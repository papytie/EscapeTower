using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyChase : MonoBehaviour, IMovement
{
    public Vector2 EnemyDirection { get; set; }
    public bool MoveCompleted { get; set; }
    public ChaseData Data { get; }

    private ChaseData data;
    private EnemyController controller;

    public void InitMove()
    {

    }

    public void InitRef(IMovementData dataRef, EnemyController controllerRef)
    {
        data = dataRef as ChaseData;
        controller = controllerRef;
    }

    public void Move()
    {
        Vector3 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        EnemyDirection = (controller.CurrentTarget.transform.position - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, controller.CurrentTarget.transform.position);

        if (targetDistance > data.minRange)
        {
            //Check for collision
            controller.Collision.MoveToCollisionCheck(EnemyDirection, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
            transform.position = finalPosition;

        }

    }


}
