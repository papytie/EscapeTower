using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]

public class EnemyCollisionComponent : MonoBehaviour
{
    public LayerMask CollisionLayer => collisionLayer;

    [Header("Collider Settings")]
    [SerializeField] float collisionMinDist = .01f;
    [SerializeField] float colliderRadius = 1;
    [SerializeField] LayerMask collisionLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.yellow;

    float checkDistance = 0;
    Vector3 debugPosition = Vector3.zero;

    private void Start()
    {
        checkDistance = collisionMinDist * 2;
        debugPosition = transform.position;
    }

    public void MoveCollisionCheck(Vector3 direction, float distance, LayerMask checkLayer, out Vector3 fixedPosition, out RaycastHit2D collision)
    {
        collision = Physics2D.CircleCast(transform.position, colliderRadius, direction, checkDistance, checkLayer);
        Vector3 moveVector = direction * distance;

        //Debug check position
        if (showDebug)
            debugPosition = transform.position + direction * checkDistance;

        //If no collision occurs then use unmodified move vector
        fixedPosition = transform.position + moveVector;

        //If a collision occurs then check for a sticky position
        if (collision)
        {
            if (showDebug)
                debugPosition = collision.centroid;

            Vector3 stickyPos = collision.centroid + collision.normal * collisionMinDist;
            Vector3 stickyToInitial = (transform.position + moveVector) - stickyPos;
            Vector3 stickyAxis = Vector2.Perpendicular(collision.normal);

            //Projection of fixed player movement vector on Sticky Axis
            Vector3 projectedPos = stickyPos + stickyAxis * Vector2.Dot(stickyAxis, stickyToInitial);

            //Check if movement should be applied instead or if it ends in collider dead zone
            if (moveVector.magnitude > (projectedPos - transform.position).magnitude || collision.distance < collisionMinDist * .9f)
            {
                fixedPosition = projectedPos;
            }

        }

    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = colliderDebugColor;
            Gizmos.DrawWireSphere(transform.position, colliderRadius);
            Gizmos.color = Color.white;

            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(debugPosition, colliderRadius);
                Gizmos.color = Color.white;

            }
        }

    }
}
