using UnityEngine;

public class EnemyDetectionComponent : MonoBehaviour
{
    public LayerMask TargetLayer => targetLayer;
    public LayerMask ObstructionLayer => obstructionLayer;

    [Header("Detection Settings")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask obstructionLayer;

    [Header("Debug")]
    public bool showDebug;
    public Color debugColor;
    
    EnemyController controller;

    public void InitRef(EnemyController ctrlRef)
    {
        controller = ctrlRef;
    }

    public bool PlayerDetection(out GameObject target)
    {
        RaycastHit2D[] detectionList = Physics2D.CircleCastAll(transform.position, controller.Stats.DetectionRadius, Vector2.zero, 0, targetLayer);
        foreach (RaycastHit2D detection in detectionList)
        {
            if (detection)
            {
                //Set player direction vector
                Vector3 toPlayerVector = detection.transform.position - transform.position;
                if (!Physics2D.Raycast(transform.position, toPlayerVector.normalized, toPlayerVector.magnitude, obstructionLayer))
                {
                    target = detection.transform.gameObject;
                    return true;
                }
            }
        }
        target = null;
        return false;
    }

    private void OnDrawGizmos()
    {
        if(showDebug)
        {
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(transform.position, controller.Stats.DetectionRadius);
            if(PlayerDetection(out GameObject target))
                Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
}
