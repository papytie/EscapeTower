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
    [SerializeField] float speed = 1;
    [SerializeField] float cooldown;
    [SerializeField] float duration;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] float debugSize = .2f;
    [SerializeField] Color debugColor = Color.red;
    
    bool isOnCooldown;
    bool isDashing;
    float cooldownEndTime;
    float dashCurrentTime;
    float dashStartTime;
    Vector3 dashTarget = Vector3.zero;
    Vector3 dashStart = Vector3.zero;

    PlayerCollision collision;
    PlayerLifeSystem lifeSystem;
    PlayerStats stats;

    public void InitRef(PlayerCollision collisionRef, PlayerLifeSystem lifeSystemRef, PlayerStats statsRef)
    {
        collision = collisionRef;
        lifeSystem = lifeSystemRef;
        stats = statsRef;
    }

    private void Update()
    {
        if (isOnCooldown && Time.time >= cooldownEndTime)
            isOnCooldown = false;

        if (isDashing)
        {
            float t = Mathf.Clamp01((Time.time - dashStartTime) / dashStartTime);

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


            if (hit || Time.time >= dashStartTime + stats.GetModifiedSecondaryStat(SecondaryStat.DashDuration))
            {
                isDashing = false;
                lifeSystem.IsInvincible = false;
                SetCooldownTimer(stats.GetModifiedSecondaryStat(SecondaryStat.DashCooldown));
            }
        }

    }

    public void DashActivation(Vector3 dir)
    {
        dashStart = transform.position;
        dashTarget = transform.position + dir.normalized * stats.GetModifiedMainStat(MainStat.DashDistance);
        SetDashTimer();
        //Player is invincible during Dash
        lifeSystem.SetInvincibleTimer(stats.GetModifiedSecondaryStat(SecondaryStat.DashDuration));
    }

    void SetCooldownTimer(float duration)
    {
        cooldownEndTime = Time.time + duration;
        isOnCooldown = true;
    }

    void SetDashTimer()
    {
        dashStartTime = Time.time;
        isDashing = true;
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
