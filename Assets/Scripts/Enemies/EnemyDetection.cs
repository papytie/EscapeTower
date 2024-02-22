using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] float detectionRadius = 5;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstructionLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color debugColor;
    
    Vector2 targetPosDebug = Vector2.zero;

    public bool PlayerDetection(out GameObject player)
    {
        RaycastHit2D[] detectionList = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, 0, playerLayer);
        foreach (RaycastHit2D detection in detectionList)
        {
            if (detection)
            {
                //Get player position for debug
                targetPosDebug = detection.transform.position;

                //Set player direction vector
                Vector3 toPlayerVector = detection.transform.position - transform.position;
                if (!Physics2D.Raycast(transform.position, toPlayerVector.normalized, toPlayerVector.magnitude, obstructionLayer))
                {
                    player = detection.transform.gameObject;
                    return true;

                }
            }
        }

        targetPosDebug = Vector2.zero;
        player = null;
        return false;
    }

    private void OnDrawGizmos()
    {
        if(showDebug && Application.isPlaying)
        {
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            if(targetPosDebug != Vector2.zero)
                Gizmos.DrawLine(transform.position, targetPosDebug);

            Gizmos.color = Color.white;

        }
    }
}
