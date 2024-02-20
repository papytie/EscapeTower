using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float enemySpeed = 2;
    [SerializeField] float minRange = 1;
    [SerializeField] float maxRange = 5;

    [SerializeField] GameObject target;

    public void SetTarget(GameObject objectToTarget)
    {
        target = objectToTarget;
    }

    public void ChaseTarget()
    {
        Vector3 thisToTarget = target.transform.position - transform.position;
        Vector3 targetDirection = thisToTarget.normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance < minRange) return;
        
        transform.position += enemySpeed * Time.deltaTime * targetDirection;
    }

    public void FleeTarget()
    {
        Vector3 targetToThis = transform.position - target.transform.position;
        Vector3 toThisDirection = targetToThis.normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance < maxRange)
            transform.position += enemySpeed * Time.deltaTime * toThisDirection;
    }

    public void StayAtRangeFromTarget()
    {
        //From this to target
        Vector3 thisToTarget = target.transform.position - transform.position;
        Vector3 targetDirection = thisToTarget.normalized;

        //From target to this
        Vector3 targetToThis = transform.position - target.transform.position;
        Vector3 toThisDirection = targetToThis.normalized;

        //Distance
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > maxRange)
            transform.position += enemySpeed * Time.deltaTime * targetDirection;

        if (targetDistance < minRange)
            transform.position += enemySpeed * Time.deltaTime * toThisDirection;

    }
}
