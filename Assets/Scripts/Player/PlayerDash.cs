using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public bool DashAvailable => !isOnCooldown && !isDashing;
    public bool IsDashing => isDashing;
    public float Cooldown => cooldown;
    public float Duration => duration;
    public float Speed => speed;
    public float Distance => distance;

    [Header("Dash Settings")]
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] float distance;
    [SerializeField] float speed;
    [SerializeField] float cooldown;
    [SerializeField] float duration;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] float debugSize = .2f;
    [SerializeField] Color debugColor = Color.red;
    
    bool isOnCooldown;
    bool isDashing;
    float coolDownTime;
    float dashCurrentTime;
    Vector3 dashTarget = Vector3.zero;
    Vector3 dashStart = Vector3.zero;

    PlayerCollision collision;
    PlayerLifeSystem lifeSystem;

    public void InitRef(PlayerCollision collisionRef, PlayerLifeSystem lifeSystemRef)
    {
        collision = collisionRef;
        lifeSystem = lifeSystemRef;
    }

    private void Update()
    {
        if (isOnCooldown && TimeUtils.CustomTimer(ref coolDownTime, cooldown))
            isOnCooldown = false;

        if (isDashing)
        {
            dashCurrentTime += Time.deltaTime;
            float t = Mathf.Clamp01(dashCurrentTime / duration);

            //Use curve to modify lerp transition
            Vector3 dashTargetPos = Vector3.Lerp(dashStart, dashTarget, dashCurve.Evaluate(t));

            //Calculate value of next Dash movement
            float dashStepValue = (dashTargetPos - transform.position).magnitude;

            //Check at next dash step position if collision occurs
            collision.MoveCollisionCheck(dashTarget.normalized, dashStepValue, collision.WallLayer, out Vector3 fixedPosition, out RaycastHit2D hit);

            if (hit)
                transform.position = fixedPosition;
            else
                transform.position = dashTargetPos;


            if (hit || TimeUtils.CustomTimer(ref dashCurrentTime, duration))
            {
                isDashing = false;
                lifeSystem.IsInvincible = false;
                dashCurrentTime = 0;
            }
        }

    }

    public void DashActivation(Vector3 dir)
    {
        dashStart = transform.position;
        dashTarget = transform.position + dir.normalized * distance;
        isDashing = true;
        isOnCooldown = true;
        //Player is invincible during Dash
        lifeSystem.IsInvincible = true;
    }

    private void OnDrawGizmos()
    {
        if (!showDebug || !isDashing) return;
        Gizmos.color = debugColor;
        Gizmos.DrawLine(transform.position, dashTarget);
        Gizmos.DrawSphere(dashTarget, debugSize);
        Gizmos.color = debugColor;
    }
}
