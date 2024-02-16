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
    List<EnemyLifeSystem> enemiesDamaged = new();

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
        if (hitboxTime >= playerWeaponSlot.EquippedWeapon.HitboxDuration)
        {
            hitboxTime = 0;
            isTrigger = false;

            //Stop Hitbox check
            CancelInvoke(nameof(HitboxDetection));
            enemiesHit.Clear();
        }
    }

    public void AttackActivation(Vector3 attackDirection)
    {
        isAttacking = true;
        isOnCooldown = true;
        isTrigger = true;

        //Rotation done in PlayerController
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, attackDirection);

        playerWeaponSlot.EquippedWeapon.WeaponAttackFX();

        //Begin Hitbox check coroutine
        InvokeRepeating(nameof(HitboxDetection), 0, playerWeaponSlot.EquippedWeapon.HitboxDelay);

    }

    public void HitboxDetection()
    {
        if (playerWeaponSlot.EquippedWeapon.WeaponHitBoxResult(transform.position.ToVector2(), transform.up, out RaycastHit2D[] collisions))
        {
            foreach(RaycastHit2D collision in collisions)
            {
                EnemyLifeSystem enemy = collision.transform.GetComponent<EnemyLifeSystem>();
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
        if (showDebug && Application.isPlaying)
        {
            Gizmos.color = colliderDebugColor;
            Gizmos.DrawWireSphere(transform.position.ToVector2() + transform.up.ToVector2() * playerWeaponSlot.EquippedWeapon.HitboxRange, playerWeaponSlot.EquippedWeapon.HitboxRadius);
            Gizmos.color = Color.white;

        }
    }

}
