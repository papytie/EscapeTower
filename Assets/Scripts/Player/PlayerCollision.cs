using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public LayerMask WallLayer => wallLayer;

    [Header("Collider Settings")]
    [SerializeField] Vector2 BoxColliderSize;
    [SerializeField] Vector3 BoxColliderPos;
    [SerializeField] LayerMask wallLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color boxDebugColor = Color.yellow;

    public void InitRef()
    {

    }

    public bool CheckCollision(Vector2 direction, float distance, LayerMask targetLayer, out Vector2 collisionPosition)
    {
        collisionPosition = Vector2.zero;
        RaycastHit2D isHit = Physics2D.BoxCast(transform.position, BoxColliderSize, 0, direction, distance, targetLayer);
        if (isHit) 
        {
            collisionPosition = isHit.centroid;
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if(showDebug)
        {
            Gizmos.color = boxDebugColor;
            Gizmos.DrawWireCube(transform.position + BoxColliderPos, BoxColliderSize);
            Gizmos.color = Color.white;
        }
    }
}
