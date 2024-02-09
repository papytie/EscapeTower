using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public LayerMask WallLayer => wallLayer;
    public float ColliderRadius => colliderRadius;
    public bool ShowDebug => showDebug;

    [Header("Collider Settings")]
    [SerializeField] float colliderRadius;
    [SerializeField] LayerMask wallLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.yellow;

    public void InitRef()
    {

    }

    public bool CircleCheckCollision(Vector2 dir, float dist, LayerMask layer, out Vector2 normal) 
    {
        normal = Vector2.zero;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, colliderRadius, dir, dist, layer);
        if(hit)
        {
            normal = hit.normal;
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
        }
    }
}
