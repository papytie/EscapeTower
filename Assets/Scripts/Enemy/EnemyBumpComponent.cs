using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBumpComponent : MonoBehaviour
{
    public bool CanMove => !isBump && !isStun;

    [Header("Bump Settings")]
    [SerializeField] AnimationCurve bumpCurve;
    [SerializeField] float bumpDistanceRatio = .1f;
    [SerializeField] float bumpDuration = .1f;
    [SerializeField] float stunDuration = .5f;

    bool isBump;
    bool isStun;
    float bumpCurrentTime;
    float stunEndTime;
    Vector3 bumpTarget = Vector3.zero;
    Vector3 bumpStart = Vector3.zero;

    EnemyCollisionComponent collision;

    public void InitRef(EnemyCollisionComponent enemyCollision)
    {
        collision = enemyCollision;

    }

    private void Update()
    {
        if (isBump)
        {
            bumpCurrentTime += Time.deltaTime;

            float t = Mathf.Clamp01(bumpCurrentTime / bumpDuration);

            //Use curve to modify lerp transition
            Vector3 bumpTargetPos = Vector3.Lerp(bumpStart, bumpTarget, bumpCurve.Evaluate(t));

            //Calculate value of next Dash movement
            float bumpStepValue = (bumpTargetPos - transform.position).magnitude;

            //Check at next dash step position if collision occurs
            collision.MoveCollisionCheck(bumpTarget.normalized, bumpStepValue, collision.CollisionLayer, out Vector3 fixedPosition, out RaycastHit2D hit);

            if (hit)
                transform.position = fixedPosition;
            else
                transform.position = bumpTargetPos;

            if (hit || bumpCurrentTime >= bumpDuration)
            {
                isBump = false;
                bumpCurrentTime = 0;
                SetStunTimer(stunDuration);
            }

        }

        if (isStun && Time.time >= stunEndTime)
            isStun = false;
    }

    public void BumpedAwayActivation(Vector3 dir, float dmg)
    {
        bumpStart = transform.position;
        bumpTarget = transform.position + dir.normalized * bumpDistanceRatio * dmg;
        isBump = true;
    }

    void SetStunTimer(float duration)
    {
        stunEndTime = Time.time + duration;
        isStun = true;
    }
}
