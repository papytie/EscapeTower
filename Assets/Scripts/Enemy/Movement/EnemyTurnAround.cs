using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyTurnAround : MonoBehaviour, IMovement
{
    public TurnAroundData MovementData { get; set; }
    public Vector2 EnemyDirection { get; set; }

    public void Init(IMovementData data)
    {
        MovementData = data as TurnAroundData;
    }

    public void Move(GameObject target, EnemyCollisionComponent collision)
    {
        EnemyDirection = (target.transform.position - transform.position).normalized;  

        //Move with collision check
        collision.MoveCollisionCheck(GetMoveDirection(EnemyDirection), MovementData.speed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
        transform.position = finalPosition;
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
}
