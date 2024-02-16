using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerDash : MonoBehaviour
{
    public bool DashAvailable => !isOnCooldown && !isDashing;
    public bool IsDashing => isDashing;

    [Header("Dash Settings")]
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] float dashDistance;
    [SerializeField] float dashCooldown;
    [SerializeField] float dashDuration;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] float debugSize = .2f;
    [SerializeField] Color debugColor = Color.red;
    
    bool isOnCooldown;
    bool isDashing;
    float coolDownTime;
    float dashCurrentDuration;
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
        if (isOnCooldown)
            DashCoolDownTimer();

        if (isDashing)
            DashLerpTimer();
    }

    public void DashActivation(Vector3 dir)
    {
        dashStart = transform.position;
        dashTarget = transform.position + dir.normalized * dashDistance;
        isDashing = true;
        isOnCooldown = true;

        //Player invincibility during Dash
        lifeSystem.IsInvincible = true;
    }

    void DashCoolDownTimer()
    {
        coolDownTime += Time.deltaTime;
        if (coolDownTime >= dashCooldown) 
        {
            coolDownTime = 0;
            isOnCooldown = false;
        }
    }
    
    void DashLerpTimer()
    {
        dashCurrentDuration += Time.deltaTime;
        float t = Mathf.Clamp01(dashCurrentDuration / dashDuration);
        
        collision.CollisionCheck(dashTarget.normalized, (dashDistance / dashDuration) * Time.deltaTime, collision.WallLayer, out Vector3 fixedPosition, out RaycastHit2D hit);

        if(hit)
            transform.position = fixedPosition;
        
        else 
            transform.position = Vector3.Lerp(dashStart, dashTarget, dashCurve.Evaluate(t)); //use curve to modify lerp transition

        if (dashCurrentDuration >= dashDuration || hit) 
        {
            isDashing = false;
            dashCurrentDuration = 0;

            //End Invincibility
            lifeSystem.IsInvincible = false;
            return;
        }
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
