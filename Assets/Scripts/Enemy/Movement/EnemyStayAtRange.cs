using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStayAtRange : MonoBehaviour, IMovement
{
    public StayAtRangeData MovementData { get; set; }
    public Animator EnemyAnimator { get; set; }
    public Vector2 EnemyDirection { get; set; }

    public void Init(IMovementData data, Animator animator)
    {
        MovementData = data as StayAtRangeData;
        EnemyAnimator = animator;
    }

    public void Move(GameObject target, EnemyCollisionComponent collision)
    {
        EnemyDirection = (target.transform.position - transform.position).normalized;

        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > MovementData.maxRange || targetDistance < MovementData.minRange)
        {
            //Invert direction if too close of target
            if (targetDistance < MovementData.minRange) EnemyDirection = -EnemyDirection;

            EnemyAnimator.SetFloat(GameParams.Animation.ENEMY_UP_FLOAT, EnemyDirection.y);
            EnemyAnimator.SetFloat(GameParams.Animation.ENEMY_RIGHT_FLOAT, EnemyDirection.x);
            //Look away/at rotation
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

            //Check for collision
            collision.MoveCollisionCheck(EnemyDirection, MovementData.speed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;
        }

    }
}
