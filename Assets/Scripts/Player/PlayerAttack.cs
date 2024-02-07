using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAttack : MonoBehaviour
{
    public bool AttackAvailable => !isOnCooldown;
    public bool IsAttacking => isAttacking;

    [Header("Debug")]
    [SerializeField] bool showDebug = false;
    [SerializeField] float debugSize = .2f;
    [SerializeField] Color debugColor = Color.green;

    PlayerInputs playerInputs;
    PlayerWeaponSlot playerWeaponSlot;

    float coolDownTime = 0;
    float attackLagTime = 0; 
    bool isOnCooldown = false;
    bool isAttacking = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (isOnCooldown)
            AttackCoolDownTimer();

        if (isAttacking)
            AttackLagTimer();
    }

    public void InitRef(PlayerInputs inputs, PlayerWeaponSlot slot)
    {
        playerInputs = inputs;
        playerWeaponSlot = slot;
    }

    void AttackCoolDownTimer()
    {
        coolDownTime += Time.deltaTime;
        if (coolDownTime >= playerWeaponSlot.EquippedWeapon.Cooldown)
        {
            coolDownTime = 0;
            isOnCooldown = false;
        }
    }
    void AttackLagTimer()
    {
        attackLagTime += Time.deltaTime;
        if (attackLagTime >= playerWeaponSlot.EquippedWeapon.Lag)
        {
            attackLagTime = 0;
            isAttacking = false;
        }
    }

    public void AttackActivation(Vector3 attackDirection)
    {
        isAttacking = true;
        isOnCooldown = true;

        Quaternion rotateDirection = Quaternion.LookRotation(Vector3.forward, attackDirection);
        transform.rotation = rotateDirection;

        playerWeaponSlot.EquippedWeapon.WeaponAttackFX();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && showDebug)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(playerInputs.MousePositionAxisInput.ReadValue<Vector2>());
            Gizmos.color = debugColor;
            Gizmos.DrawSphere(mouseWorldPos, debugSize);
            Gizmos.DrawLine(transform.position, mouseWorldPos);
            Gizmos.color = Color.white;
        }
    }

}
