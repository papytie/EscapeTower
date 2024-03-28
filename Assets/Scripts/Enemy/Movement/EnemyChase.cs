using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour, IMovement
{
    private ChaseData movementData;
    public Animator EnemyAnimator { get; set; }
    public Vector2 EnemyDirection { get; set; }

    public void Init(IMovementData data, Animator animator)
    {
        movementData = data as ChaseData;
        EnemyAnimator = animator;
    }

    public void Move(GameObject target, EnemyCollisionComponent collision)
    {
        EnemyDirection = (target.transform.position - transform.position).normalized;
        EnemyAnimator.SetFloat(GameParams.Animation.ENEMY_UP_FLOAT, EnemyDirection.y);
        EnemyAnimator.SetFloat(GameParams.Animation.ENEMY_RIGHT_FLOAT, EnemyDirection.x);

        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > movementData.minRange)
        {
            //Look At Rotation
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

            //Check for collision
            collision.MoveCollisionCheck(EnemyDirection, movementData.speed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;
        }

    }

}
