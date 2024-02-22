using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlee : MonoBehaviour
{
    [Header("Flee Settings")]
    [SerializeField] float moveSpeed = 3;
    [SerializeField] float maxRange = 5;

    EnemyCollision collision;

    public void InitRef(EnemyCollision enemyCollision)
    {
        collision = enemyCollision;

    }

    public void FleeTarget(GameObject target)
    {
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);


        if (targetDistance < maxRange)
        {
            //Look away rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, -targetDirection);

            //Check for collision
            collision.MoveCollisionCheck(-targetDirection, moveSpeed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;

        }

    }
}