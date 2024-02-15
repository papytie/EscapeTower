using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAttack : MonoBehaviour
{
    public bool AttackAvailable => !isOnCooldown;
    public bool IsAttacking => isAttacking;

    float coolDownTime = 0;
    float attackLagTime = 0; 
    bool isOnCooldown = false;
    bool isAttacking = false;

    PlayerInputs playerInputs;
    PlayerWeaponSlot playerWeaponSlot;

    public void InitRef(PlayerInputs inputs, PlayerWeaponSlot slot)
    {
        playerInputs = inputs;
        playerWeaponSlot = slot;
    }

    void Update()
    {
        if (isOnCooldown)
            AttackCoolDownTimer();

        if (isAttacking)
            AttackLagTimer();
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

        transform.rotation = Quaternion.LookRotation(Vector3.forward, attackDirection);

        playerWeaponSlot.EquippedWeapon.WeaponAttackFX();

        //Hitbox System
        if (playerWeaponSlot.EquippedWeapon.WeaponHitBoxResult(attackDirection, out RaycastHit2D[] collisions))
        {
            foreach(RaycastHit2D collision in collisions)
            {
                collision.transform.GetComponent<EnemyLifeSystem>().TakeDamage(playerWeaponSlot.EquippedWeapon.Damage);

            }
        }
    }

    
    
}
