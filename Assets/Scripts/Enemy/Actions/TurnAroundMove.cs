using System.Collections.Generic;
using UnityEngine;

public class TurnAroundMove : MonoBehaviour, IAction
{
    public bool IsCompleted { get; set; }
    public bool IsAvailable => throw new System.NotImplementedException();
    public Vector3 Direction => direction;
    Vector2 direction;

    private TurnAroundData data;
    private EnemyController controller;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as TurnAroundData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
    }

    public void UpdateProcess()
    {
        Vector2 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        direction = (controller.CurrentTargetPos - offsetPosition).normalized;

        //Move with collision check
        controller.Collision.MoveToCollisionCheck(GetMoveDirection(direction), controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
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

    public void EndProcess()
    {
    }
}
