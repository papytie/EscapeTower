using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyTurnAround : MonoBehaviour, IMovement
{
    public Vector2 EnemyDirection { get; set; }
    public bool MoveCompleted { get; set; }
    public TurnAroundData Data { get; }

    private TurnAroundData data;
    private EnemyController controller;

    public void InitMove()
    {
        throw new System.NotImplementedException();
    }

    public void InitRef(IMovementData dataRef, EnemyController controllerRef)
    {
        data = dataRef as TurnAroundData;
        controller = controllerRef;
    }

    public void Move()
    {
        Vector3 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        EnemyDirection = (controller.CurrentTarget.transform.position - offsetPosition).normalized;

        //Move with collision check
        controller.Collision.MoveToCollisionCheck(GetMoveDirection(EnemyDirection), controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
        transform.position = finalPosition;

    }

    Vector2 GetMoveDirection(Vector2 targetDirection)
    {
        return data.direction switch
        {
            TurnDirection.Clockwise => Quaternion.Euler(0, 0, 90) * targetDirection,
            TurnDirection.Anticlockwise => Quaternion.Euler(0, 0, -90) * targetDirection,
            _ => Vector2.zero,
        };
    }
}
