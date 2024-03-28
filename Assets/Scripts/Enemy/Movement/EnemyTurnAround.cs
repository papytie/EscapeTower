using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyTurnAround : MonoBehaviour, IMovement
{
    public TurnAroundData MovementData { get; set; }
    public Animator EnemyAnimator { get; set; }
    public Vector2 EnemyDirection { get; set; }

    public void Init(IMovementData data, Animator animator)
    {
        MovementData = data as TurnAroundData;
        EnemyAnimator = animator;
    }

    public void Move(GameObject target, EnemyCollisionComponent collision)
    {
        EnemyDirection = (target.transform.position - transform.position).normalized;  
        EnemyAnimator.SetFloat(GameParams.Animation.ENEMY_UP_FLOAT, EnemyDirection.y);
        EnemyAnimator.SetFloat(GameParams.Animation.ENEMY_RIGHT_FLOAT, EnemyDirection.x);

        //Move with collision check
        collision.MoveCollisionCheck(GetMoveDirection(EnemyDirection), MovementData.speed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
        transform.position = finalPosition;
        
        //Look rotation
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, GetLookDirection(targetDirection));

    }

    Vector2 GetMoveDirection(Vector2 targetDirection)
    {
        return MovementData.direction switch
        {
            TurnDirection.Clockwise => Quaternion.Euler(0, 0, 90) * targetDirection,
            TurnDirection.Anticlockwise => Quaternion.Euler(0, 0, -90) * targetDirection,
            _ => Vector2.zero,
        };
    }

    Vector2 GetLookDirection(Vector2 targetDirection) 
    {

        return MovementData.look switch
        {
            LookDirection.Movement => GetMoveDirection(targetDirection),
            LookDirection.Target => targetDirection,
            LookDirection.TargetInvert => -targetDirection,
            LookDirection.MovementInvert => -GetMoveDirection(targetDirection),
            _ => Vector2.zero,
        };

    }
}
