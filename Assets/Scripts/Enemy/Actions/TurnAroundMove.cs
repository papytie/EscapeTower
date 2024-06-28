using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TurnAroundMove : MonoBehaviour, IAction
{
    public Vector2 MoveDirection { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsAvailable => throw new System.NotImplementedException();

    private TurnAroundData data;
    private EnemyController controller;

    public void StartProcess()
    {
        throw new System.NotImplementedException();
    }

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as TurnAroundData;
        controller = controllerRef;
    }

    public void UpdateProcess()
    {
        Vector3 offsetPosition = transform.position.ToVector2() + controller.CircleCollider.offset;
        MoveDirection = (controller.CurrentTarget.transform.position - offsetPosition).normalized;

        //Move with collision check
        controller.Collision.MoveToCollisionCheck(GetMoveDirection(MoveDirection), controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
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
        throw new System.NotImplementedException();
    }
}
