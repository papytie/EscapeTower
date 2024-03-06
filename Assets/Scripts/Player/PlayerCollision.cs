using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCollision : MonoBehaviour
{
    public LayerMask WallLayer => wallLayer;
    public LayerMask EnemyLayer => enemyLayer;
    public LayerMask PickupLayer => pickupLayer;
    public float ColliderRadius => colliderRadius;
    public bool ShowDebug => showDebug;

    [Header("Collider Settings")]
    [SerializeField] float collisionMinDist = .01f;
    [SerializeField] float colliderRadius = 1;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask pickupLayer;

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

    public bool EnemyCheckCollision(LayerMask layer, out int damage)
    {
        damage = 0;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, colliderRadius, Vector2.zero, 0, layer);
        if (hit && !hit.transform.GetComponent<EnemyLifeSystem>().IsDead)
        {
            damage = hit.transform.GetComponent<EnemyAttack>().BaseDamage;
            return true;
        }
        return false;

    }

    public bool PickupCheckCollision(LayerMask layer, out PickupItem item)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, colliderRadius, Vector2.zero, 0, layer);
        if (hit)
        {
            item = hit.transform.GetComponent<PickupItem>();
            return true;
        }
        item = null;
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
