using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public bool DashAvailable => !isOnCooldown && !isDashing;
    public bool IsDashing => isDashing;
    public float Cooldown => cooldown;
    public float Duration => duration;
    public float Distance => distance;

    [Header("Dash Settings")]
    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField] AnimationCurve decelerationCurve;
    [SerializeField] float distance;
    [SerializeField] float cooldown;
    [SerializeField] float duration;
    [SerializeField] float dashMaxSpeedFactor = 2;
    [SerializeField, Range(0,1)] float decelerationTiming = .5f;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] float debugSize = .2f;
    [SerializeField] Color debugColor = Color.red;
    
    bool isOnCooldown;
    bool isDashing;

    float cooldownEndTime;
    float dashDuration;
    float startDashTime;
    float startDeceleration;
    //float currentSpeed;
    float moveSpeed;

    Vector3 dashTarget = Vector3.zero;
    Vector3 dashStart = Vector3.zero;
    Vector3 dashDirection = Vector3.zero;

    PlayerController controller;

    public void InitRef(PlayerController playerController)
    {
        controller = playerController;
    }

    private void Update()
    {
        if (isOnCooldown && Time.time >= cooldownEndTime)
            isOnCooldown = false;

        if (isDashing)
        {
            //float t = Mathf.Clamp01((Time.time - startDashTime) / controller.Stats.GetModifiedSecondaryStat(SecondaryStat.DashDuration));

            //Use curve to modify lerp transition
            //Vector3 dashTargetPos = Vector3.Lerp(dashStart, dashTarget, dashCurve.Evaluate(t));

            //Calculate value of next Dash movement
            //float dashStepValue = (dashTargetPos - transform.position).magnitude;

            float dashMaxSpeed = moveSpeed * dashMaxSpeedFactor;
            float currentDashSpeed = moveSpeed;
            float t;

            if(Time.time > startDashTime && Time.time < startDeceleration)
            {
                t = (Time.time - startDashTime) / (dashDuration * decelerationTiming); 
                currentDashSpeed = Mathf.Lerp(moveSpeed, dashMaxSpeed, accelerationCurve.Evaluate(t));
            }
            if(Time.time > startDeceleration)
            {
                t = (Time.time - startDeceleration) / (dashDuration * (1 - decelerationTiming)); 
                currentDashSpeed = Mathf.Lerp(dashMaxSpeed, moveSpeed, decelerationCurve.Evaluate(t));
            }

            //Check at next dash step position if collision occurs
            controller.Collision.MoveToCollisionCheck(dashDirection, currentDashSpeed * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 fixedPosition, out List<RaycastHit2D> hitList);

            if (hitList.Count > 0)
                transform.position = fixedPosition;
            else
                transform.position += dashDirection * currentDashSpeed * Time.deltaTime;

            //Debug.Log($"{Time.frameCount} - Dash Movement : " + currentDashSpeed);

            if (hitList.Count > 0 || Time.time >= startDashTime + dashDuration)
            {
                isDashing = false;
                cooldownEndTime = Time.time + controller.Stats.GetModifiedSecondaryStat(SecondaryStat.DashCooldown);
                isOnCooldown = true;
            }
        }
    }

    public void DashActivation(Vector3 dir)
    {
        //dashStart = transform.position;
        //dashTarget = transform.position + dir.normalized * controller.Stats.GetModifiedMainStat(MainStat.DashDistance);
        moveSpeed = controller.Stats.GetModifiedMainStat(MainStat.MoveSpeed);
        dashDuration = controller.Stats.GetModifiedSecondaryStat(SecondaryStat.DashDuration);
        dashDirection = dir;
        startDashTime = Time.time;
        startDeceleration = Time.time + dashDuration * decelerationTiming;
        isDashing = true;
        //Player is invincible during Dash
        controller.LifeSystem.StartInvincibility(dashDuration);
    }

    private void OnDrawGizmos()
    {
/*        if (!showDebug || !isDashing) return;
        Gizmos.color = debugColor;
        Gizmos.DrawLine(transform.position, dashTarget);
        Gizmos.DrawSphere(dashTarget, debugSize);
        Gizmos.color = debugColor;
*/    }
}
