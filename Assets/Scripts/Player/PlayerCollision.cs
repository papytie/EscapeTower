using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCollision : MonoBehaviour
{
    public LayerMask WallLayer => wallLayer;
    public LayerMask EnemyLayer => enemyLayer;
    public float ColliderRadius => colliderRadius;
    public bool ShowDebug => showDebug;

    [Header("Collider Settings")]
    [SerializeField] float collisionMinDist = .01f;
    [SerializeField] float colliderRadius = 1;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask enemyLayer;

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

    public void CollisionCheck(Vector3 direction, float distance, LayerMask checkLayer, out Vector3 finalPosition)
    {
        RaycastHit2D collision = Physics2D.CircleCast(transform.position, colliderRadius, direction, checkDistance, checkLayer);
        Vector3 moveVector = distance * direction;

        //Debug check position
        if (showDebug)
            debugPosition = transform.position + direction * checkDistance;

        //If no collision occurs then use unmodified move vector
        finalPosition = transform.position + moveVector;

        //If a collision occurs then check for a sticky position
        if (collision)
        {
            Vector3 stickyPos = collision.centroid + collision.normal * collisionMinDist;
            Vector3 stickyToInitial = (transform.position + moveVector) - stickyPos;
            Vector3 stickyAxis = Vector2.Perpendicular(collision.normal);

            //Projection of fixed player movement vector on Sticky Axis
            Vector3 projectedPos = stickyPos + stickyAxis * Vector2.Dot(stickyAxis, stickyToInitial);

            if (moveVector.magnitude > (projectedPos - transform.position).magnitude || collision.distance < collisionMinDist * .9f)
            {
                finalPosition = stickyPos + stickyAxis * Vector2.Dot(stickyAxis, stickyToInitial);
            }

        }

    }

    public bool EnemyCheckCollision(LayerMask layer, out int damage)
    {
        damage = 0;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, colliderRadius, Vector2.zero, 0, layer);
        if (hit)
        {
            damage = hit.transform.GetComponent<EnemyAttack>().BaseDamage;
            return true;
        }
        return false;

    }

    private void OnDrawGizmos()
    {
        if(showDebug)
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
