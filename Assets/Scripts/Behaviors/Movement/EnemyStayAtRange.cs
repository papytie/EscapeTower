using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStayAtRange : MonoBehaviour, IMovement
{
    public StayAtRangeData MovementData { get; set; }

    public void Init(IMovementData data)
    {
        MovementData = data as StayAtRangeData;
    }

    public void Move(GameObject target, EnemyCollisionComponent collision)
    {
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > MovementData.maxRange || targetDistance < MovementData.minRange)
        {
            //Invert direction if too close of target
            if (targetDistance < MovementData.minRange) targetDirection = -targetDirection;

            //Look away/at rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

            //Check for collision
            collision.MoveCollisionCheck(targetDirection, MovementData.speed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;
        }

    }
}
