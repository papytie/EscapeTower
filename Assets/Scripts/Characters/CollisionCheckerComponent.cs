using UnityEngine;
using Dest.Math;
using System.Collections.Generic;

public class CollisionCheckerComponent : MonoBehaviour
{
    public LayerMask BlockingObjectsLayer => blockingObjectsLayer;
    public LayerMask IntractionObjectsLayer => intractionObjectsLayer;
    public float ColliderRadius => circleColliderRadius;
    public bool ShowDebug => showDebug;

    //[Header("Collider Settings"), Space]

    //[Header("Shape")]
    [SerializeField] ColliderShape colliderShape = ColliderShape.Circle;
    [SerializeField] float circleColliderRadius = 1;
    [SerializeField] Vector2 boxColliderSize = new(.1f,.1f);

    //[Header("Settings")]
    [SerializeField] float collisionMinDist = .01f;

    //[Header("Layers"), Space]
    [SerializeField] LayerMask blockingObjectsLayer;
    [SerializeField] LayerMask intractionObjectsLayer;

    //[Header("Debug"), Space]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.yellow;

    float checkDistance = 0;
    Vector3 debugPosition = Vector3.zero;

    public void Init()
    {
        checkDistance = collisionMinDist * 2;
        debugPosition = transform.position.ToVector2();
    }

    public void MoveToCollisionCheck(Vector3 direction, float distance, LayerMask blockLayer, out Vector3 fixedPosition, out List<RaycastHit2D> collisionListChecked)
    {
        collisionListChecked = new();
        Vector3 moveVector = direction * distance;

        //If no collision occurs then use unmodified move vector
        fixedPosition = transform.position + moveVector;
        
        //Debug check position
        if (showDebug)
            debugPosition = transform.position + direction * checkDistance;
        
        //Detection process
        RaycastHit2D[] collisionsList = colliderShape switch
        {
            ColliderShape.Box => Physics2D.BoxCastAll(transform.position, boxColliderSize, 0f, direction, checkDistance, blockLayer),
            _ => Physics2D.CircleCastAll(transform.position, circleColliderRadius, direction, checkDistance, blockLayer),
        };

        //Remove collision with self from the RaycastHit2D[]
        foreach (RaycastHit2D collision in collisionsList)
        {
            if (collision.transform.gameObject != gameObject)
                collisionListChecked.Add(collision);
        }

        //If only one collision occurs then check for a sticky position near the collided object
        if (collisionListChecked.Count == 1)
        {
            Vector3 stickyPos = collisionListChecked[0].centroid + collisionListChecked[0].normal * collisionMinDist;
            Vector3 stickyToInitial = (transform.position + moveVector) - stickyPos;
            Vector3 stickyAxis = Vector2.Perpendicular(collisionListChecked[0].normal);

            //Projection of fixed player movement vector on Sticky Axis
            Vector3 projectedPos = stickyPos + stickyAxis * Vector2.Dot(stickyAxis, stickyToInitial);

            //Check if movement should be applied instead or if it ends in collider dead zone
            if (moveVector.magnitude > (projectedPos - transform.position).magnitude || collisionListChecked[0].distance < collisionMinDist * .9f)
            {
                fixedPosition = projectedPos;
            }
        }

        if (collisionListChecked.Count > 1)
        {
            //Create 2 Lines from two first collisions
            Line2 firstCollision = Line2.CreateFromTwoPoints(collisionListChecked[0].point, collisionListChecked[0].centroid);
            Line2 secondCollision = Line2.CreateFromTwoPoints(collisionListChecked[1].point, collisionListChecked[1].centroid);

            //Create 2 Perpendicular Lines
            Line2 firstCollisionPerp = Line2.CreatePerpToLineTrhoughPoint(firstCollision, collisionListChecked[0].centroid + collisionListChecked[0].normal * collisionMinDist);
            Line2 secondCollisionPerp = Line2.CreatePerpToLineTrhoughPoint(secondCollision, collisionListChecked[1].centroid + collisionListChecked[1].normal * collisionMinDist);

            //Get the intersection of those two Perpendicular
            Intersection.FindLine2Line2(ref firstCollisionPerp, ref secondCollisionPerp, out Line2Line2Intr closestPosition);

            Vector2 toClosestPosVector = closestPosition.Point - transform.position.ToVector2();

            if (toClosestPosVector.magnitude > moveVector.magnitude)
                fixedPosition = transform.position.ToVector2() + toClosestPosVector.normalized * moveVector.magnitude;
            else fixedPosition = closestPosition.Point;
        }

    }

    public bool ObjectTriggerCheck(LayerMask layerMaskToCheck, out RaycastHit2D collisionHit)
    {
        collisionHit = colliderShape switch
        {
            ColliderShape.Box => Physics2D.BoxCast(transform.position.ToVector2(), boxColliderSize, 0, Vector2.zero, 0, layerMaskToCheck),
            _ => Physics2D.CircleCast(transform.position.ToVector2(), circleColliderRadius, Vector2.zero, 0, layerMaskToCheck),
        };
        return collisionHit ? true : false;
    }

    private void OnDrawGizmos()
    {
        if(showDebug)
        {
            Vector3 basePosition = transform.position.ToVector2();

            switch (colliderShape)
            {
                default:
                case ColliderShape.Circle:
                    Gizmos.color = colliderDebugColor;
                    Gizmos.DrawWireSphere(basePosition, circleColliderRadius);
                    Gizmos.color = Color.white;

                    if (Application.isPlaying)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireSphere(debugPosition, circleColliderRadius);
                        Gizmos.color = Color.white;
                    }
                    break;

                case ColliderShape.Box:
                    Gizmos.color = colliderDebugColor;
                    Gizmos.DrawWireCube(basePosition, boxColliderSize);
                    Gizmos.color = Color.white;

                    if (Application.isPlaying)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(debugPosition, boxColliderSize);
                        Gizmos.color = Color.white;
                    }
                    break;
            }
        }

    }

}

public enum ColliderShape
{
    Circle = 0,
    Box = 1,
}
