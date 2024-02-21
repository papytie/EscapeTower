using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool CanMove => !isBump && !isStun;

    [Header("Movement Settings")]
    [SerializeField] float enemySpeed = 2;
    [SerializeField] float minRange = 1;
    [SerializeField] float maxRange = 5;
    [Header("Bump Settings")]
    [SerializeField] float bumpDistance = 1;
    [SerializeField] float bumpDuration = .1f;
    [SerializeField] AnimationCurve bumpCurve;
    [Header("Stun Settings")]
    [SerializeField] float stunDuration = .5f;

    bool isBump;
    bool isStun;
    float bumpCurrentDuration;
    float stunCurrentDuration;
    Vector3 bumpTarget = Vector3.zero;
    Vector3 bumpStart = Vector3.zero;

    GameObject target;
    EnemyCollision collision;

    public void InitRef(EnemyCollision enemyCollision)
    {
        collision = enemyCollision;

    }

    public void SetTarget(GameObject objectToTarget)
    {
        target = objectToTarget;
    }

    private void Update()
    {
        if (target == null) return;
        
        if (isBump)
            BumpUpdate();

        if (isStun)
            StunTimer();
    }

    public void ChaseTarget()
    {
        Vector2 TargetDirection = (target.transform.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > minRange)
        {
            collision.MoveCollisionCheck(TargetDirection, enemySpeed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;
        }

    }

    public void FleeTarget()
    {
        Vector2 TargetDirection = (target.transform.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance < maxRange)
        {
            collision.MoveCollisionCheck(-TargetDirection, enemySpeed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;
        }

    }

    public void StayAtRangeFromTarget()
    {
        Vector2 TargetDirection = (target.transform.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance > maxRange || targetDistance < minRange)
        {
            if (targetDistance < minRange) TargetDirection = -TargetDirection;

            collision.MoveCollisionCheck(TargetDirection, enemySpeed * Time.deltaTime, collision.CollisionLayer, out Vector3 finalPosition, out RaycastHit2D hit);
            transform.position = finalPosition;
        }

    }
    public void LookAtTarget()
    {
        Vector2 TargetDirection = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, TargetDirection);
    }

    public void BumpedAwayActivation(Vector3 dir)
    {
        bumpStart = transform.position;
        bumpTarget = transform.position + dir.normalized * bumpDistance;
        isBump = true;
    }

    void BumpUpdate()
    {
        bumpCurrentDuration += Time.deltaTime;
        float t = Mathf.Clamp01(bumpCurrentDuration / bumpDuration);

        //Use curve to modify lerp transition
        Vector3 dashTargetPos = Vector3.Lerp(bumpStart, bumpTarget, bumpCurve.Evaluate(t));

        //Calculate value of next Dash movement
        float dashStepValue = (dashTargetPos - transform.position).magnitude;

        //Check at next dash step position if collision occurs
        collision.MoveCollisionCheck(bumpTarget.normalized, dashStepValue, collision.CollisionLayer, out Vector3 fixedPosition, out RaycastHit2D hit);

        if (hit)
            transform.position = fixedPosition;
        else
            transform.position = dashTargetPos;

        //Reset dash when Dash duration end or Collision hit
        if (bumpCurrentDuration >= bumpDuration || hit)
        {
            isBump = false;
            isStun = true;
            bumpCurrentDuration = 0;
        }
    }

    void StunTimer()
    {
        stunCurrentDuration += Time.deltaTime;
        if(stunCurrentDuration >= stunDuration) 
        {
            stunCurrentDuration = 0;
            isStun = false;
        }
    }
}
