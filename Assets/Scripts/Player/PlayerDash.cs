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

    PlayerInputs playerInputs;

    public void InitRef(PlayerInputs inputRef)
    {
        playerInputs = inputRef;
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
        dashTarget = transform.position + dir * dashDistance;
        isDashing = true;
        isOnCooldown = true;
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
        
        //t = Mathf.Sin(t * Mathf.PI * 0.5f); // Formula to "ease out" the interpolation

        transform.position = Vector3.Lerp(dashStart, dashTarget, dashCurve.Evaluate(t)); //use curve to modify lerp transition

        if (dashCurrentDuration >= dashDuration) 
        {
            isDashing = false;
            dashCurrentDuration = 0;
            return;
        }
    }

    /*void DashTowards()
    {
        float dashStep = dashDistance/dashDuration * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashStep);
        float dist = Vector3.Distance(transform.position, dashTarget);
    }*/  

    private void OnDrawGizmos()
    {
        if (!showDebug || !isDashing) return;
        Gizmos.color = debugColor;
        Gizmos.DrawLine(transform.position, dashTarget);
        Gizmos.DrawSphere(dashTarget, debugSize);
        Gizmos.color = debugColor;
    }
}
