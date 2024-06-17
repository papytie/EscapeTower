using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnAround : MonoBehaviour, IMovement
{
    public TurnAroundData MovementData { get; set; }
    public Vector2 EnemyDirection { get; set; }

    public void Init(IMovementData data)
    {
        MovementData = data as TurnAroundData;
    }

    public void Move(GameObject target, CollisionCheckerComponent collision, CircleCollider2D collider, float moveSpeed)
    {
        Vector3 offsetPosition = transform.position.ToVector2() + collider.offset;
        EnemyDirection = (target.transform.position - offsetPosition).normalized;

        //Move with collision check
        collision.MoveToCollisionCheck(GetMoveDirection(EnemyDirection), moveSpeed * MovementData.speedMult * Time.deltaTime, collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
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
