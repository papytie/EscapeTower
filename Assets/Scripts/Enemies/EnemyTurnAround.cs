using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyTurnAround : MonoBehaviour, IMovement
{
    public TurnAroundData MovementData { get; set; }

    public void Init(IMovementData data)
    {
        MovementData = data as TurnAroundData;
    }

    public void Move(GameObject target, EnemyCollision collision)
    {
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;       

        //Move with collision check
        collision.MoveCollisionCheck(GetMoveDirection(targetDirection), MovementData.speed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
        transform.position = finalPosition;
        
        //Look rotation
        transform.rotation = Quaternion.LookRotation(Vector3.forward, GetLookDirection(targetDirection));

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
