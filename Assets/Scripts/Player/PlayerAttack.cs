using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAttack : MonoBehaviour
{
    public bool AttackAvailable => !isOnCooldown;
    public bool IsAttacking => isAttacking;
    public bool IsTrigger => isTrigger;
    public bool AutoAttackOnStick => autoAttackOnStick;

    [Header("Attack Settings")]
    [SerializeField] bool autoAttackOnStick = true;
    
    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.red;

    float coolDownTime = 0;
    bool isOnCooldown = false;
    float attackLagTime = 0; 
    bool isAttacking = false;
    float hitboxTime = 0; 
    bool isTrigger = false;

    List<EnemyLifeSystem> enemiesHit = new();

    PlayerWeaponSlot playerWeaponSlot;

    public void InitRef(PlayerWeaponSlot slot)
    {
        playerWeaponSlot = slot;
    }

    void Update()
    {
        if (isOnCooldown)
            AttackCoolDownTimer();

        if (isAttacking)
            AttackLagTimer();

        if (isTrigger)
            AttackHitboxTimer();
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

    void AttackHitboxTimer()
    {
        hitboxTime += Time.deltaTime;
        HitboxDetection();
        if (hitboxTime >= playerWeaponSlot.EquippedWeapon.HitboxDuration)
        {
            isTrigger = false;
            hitboxTime = 0;
            enemiesHit.Clear();
        }
    }

    public void AttackActivation()
    {
        isAttacking = true;
        isOnCooldown = true;
        isTrigger = true;
        playerWeaponSlot.EquippedWeapon.WeaponAttackFX();
    }

    public void HitboxDetection()
    {
        if (playerWeaponSlot.EquippedWeapon.WeaponHitBoxResult(transform.position.ToVector2(), transform.up, out RaycastHit2D[] collisions))
        {
            foreach(RaycastHit2D collision in collisions)
            {
                //Cast to enemy script
                EnemyLifeSystem enemy = collision.transform.GetComponent<EnemyLifeSystem>();

                //Check enemy then apply damages and add to list
                if (enemy && !enemiesHit.Contains(enemy))
                {
                    enemy.TakeDamage(playerWeaponSlot.EquippedWeapon.Damage);
                    enemiesHit.Add(enemy);

                }

            }

        }

    }

    private void OnDrawGizmos()
    {
        if (showDebug && Application.isPlaying && isTrigger)
        {
            Gizmos.color = colliderDebugColor;
            Gizmos.DrawWireSphere(transform.position.ToVector2() + transform.up.ToVector2() * playerWeaponSlot.EquippedWeapon.HitboxRange, playerWeaponSlot.EquippedWeapon.HitboxRadius);
            Gizmos.color = Color.white;

        }
    }

}
