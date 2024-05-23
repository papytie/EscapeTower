using System.Collections.Generic;
using UnityEngine;

public class EnemyFlee : MonoBehaviour, IMovement
{
    public FleeData MovementData { get; set; }
    public Vector2 EnemyDirection { get; set; }

    public void Init(IMovementData data)
    {
        MovementData = data as FleeData;
    }

    public void Move(GameObject target, CollisionCheckerComponent collision, CircleCollider2D collider)
    {
        Vector3 offsetPosition = transform.position.ToVector2() + collider.offset;
        EnemyDirection = (target.transform.position - offsetPosition).normalized;

        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance < MovementData.maxRange)
        {
            //Look away rotation
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, -targetDirection);

            //Check for collision
            collision.MoveToCollisionCheck(-EnemyDirection, MovementData.speed * Time.deltaTime, collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
            transform.position = finalPosition;

        }
    }
}
