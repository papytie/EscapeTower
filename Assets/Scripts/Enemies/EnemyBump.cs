using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBump : MonoBehaviour
{
    public bool CanMove => !isBump && !isStun;

    [Header("Bump Settings")]
    [SerializeField] AnimationCurve bumpCurve;
    [SerializeField] float bumpDistance = 1;
    [SerializeField] float bumpDuration = .1f;
    [SerializeField] float stunDuration = .5f;

    bool isBump;
    bool isStun;
    float bumpCurrentDuration;
    float stunCurrentDuration;
    Vector3 bumpTarget = Vector3.zero;
    Vector3 bumpStart = Vector3.zero;

    EnemyCollision collision;

    public void InitRef(EnemyCollision enemyCollision)
    {
        collision = enemyCollision;

    }

    private void Update()
    {
        if (isBump)
        {
            float t = Mathf.Clamp01(bumpCurrentDuration / bumpDuration);

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

            if (hit || TimeUtils.CustomTimer(ref bumpCurrentDuration, bumpDuration))
            {
                isBump = false;
                isStun = true;
            }

        }

        if (isStun && TimeUtils.CustomTimer(ref stunCurrentDuration, stunDuration))
            isStun = false;
    }

    public void BumpedAwayActivation(Vector3 dir)
    {
        bumpStart = transform.position;
        bumpTarget = transform.position + dir.normalized * bumpDistance;
        isBump = true;
    }

}
