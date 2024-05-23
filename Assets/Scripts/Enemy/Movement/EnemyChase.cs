using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour, IMovement
{
    private ChaseData movementData;
    public Vector2 EnemyDirection { get; set; }

    public void Init(IMovementData data)
    {
        movementData = data as ChaseData;
    }

    public void Move(GameObject target, CollisionCheckerComponent collision, CircleCollider2D collider)
    {
        Vector3 offsetPosition = transform.position.ToVector2() + collider.offset;
        EnemyDirection = (target.transform.position - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > movementData.minRange)
        {
            //Look At Rotation
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

            //Check for collision
            collision.MoveToCollisionCheck(EnemyDirection, movementData.speed * Time.deltaTime, collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
            transform.position = finalPosition;
        }

    }


}
