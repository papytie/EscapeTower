using UnityEngine;

public class EnemyDetectionComponent : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstructionLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color debugColor;
    
    EnemyStatsComponent stats;

    public void InitRef(EnemyStatsComponent statsRef)
    {
        stats = statsRef;
    }

    public bool PlayerDetection(out GameObject player)
    {
        RaycastHit2D[] detectionList = Physics2D.CircleCastAll(transform.position, stats.DetectionRadius, Vector2.zero, 0, playerLayer);
        foreach (RaycastHit2D detection in detectionList)
        {
            if (detection)
            {
                //Set player direction vector
                Vector3 toPlayerVector = detection.transform.position - transform.position;
                if (!Physics2D.Raycast(transform.position, toPlayerVector.normalized, toPlayerVector.magnitude, obstructionLayer))
                {
                    player = detection.transform.gameObject;
                    return true;
                }
            }
        }
        player = null;
        return false;
    }

    private void OnValidate()
    {
        stats = gameObject.GetComponent<EnemyStatsComponent>();
    }

    private void OnDrawGizmos()
    {
        if(showDebug)
        {
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(transform.position, stats.DetectionRadius);
            Gizmos.color = Color.white;

        }
    }
}
