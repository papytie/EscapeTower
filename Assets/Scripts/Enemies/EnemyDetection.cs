using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] float detectionRadius = 5;
    [SerializeField] LayerMask detectionLayer;

    public bool PlayerDetection(out GameObject player)
    {
        RaycastHit2D[] detectionList = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, 0, detectionLayer);
        foreach (RaycastHit2D detection in detectionList)
        {
            if (detection.transform.GetComponent<PlayerController>())
            {
                player = detection.transform.gameObject; 
                return true;
            }
        }
        player = null;
        return false;
    }
}
