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
            BumpUpdate();

        if (isStun)
            StunTimer();
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
        if (stunCurrentDuration >= stunDuration)
        {
            stunCurrentDuration = 0;
            isStun = false;
        }
    }
}
