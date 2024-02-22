using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStayAtRange : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float minRange = 1;
    [SerializeField] float maxRange = 5;

    EnemyCollision collision;

    public void InitRef(EnemyCollision enemyCollision)
    {
        collision = enemyCollision;

    }

    public void StayAtRangeFromTarget(GameObject target)
    {
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > maxRange || targetDistance < minRange)
        {
            //Invert direction if too close of target
            if (targetDistance < minRange) targetDirection = -targetDirection;

            //Look away/at rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

            //Check for collision
            collision.MoveCollisionCheck(targetDirection, moveSpeed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;
        }

    }
}
