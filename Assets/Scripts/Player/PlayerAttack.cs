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
    public float AttackSpeed => attackSpeed;

    [Header("Attack Settings")]
    [SerializeField] bool autoAttackOnStick = true;
    [SerializeField] float attackSpeed = 1;

    float coolDownTime = 0;
    bool isOnCooldown = false;
    float attackLagTime = 0; 
    bool isAttacking = false;
    float hitboxTime = 0; 
    bool isTrigger = false;

    List<EnemyLifeSystem> enemiesHit = new();

    PlayerWeaponSlot playerWeaponSlot;
    PlayerStats stats;

    public void InitRef(PlayerWeaponSlot weaponSlotRef, PlayerStats statsRef)
    {
        playerWeaponSlot = weaponSlotRef;
        stats = statsRef;
    }

    void Update()
    {
        if (isOnCooldown && TimeUtils.CustomTimer(ref coolDownTime, stats.GetModifiedSecondaryStat(SecondaryStat.AttackCooldown)))
            isOnCooldown = false;
        
        if (isAttacking && TimeUtils.CustomTimer(ref attackLagTime, stats.GetModifiedSecondaryStat(SecondaryStat.AttackLag)))
            isAttacking = false;

        if (isTrigger)
            HitboxDetection();
            if(TimeUtils.CustomTimer(ref hitboxTime, playerWeaponSlot.EquippedWeapon.HitboxDuration))
            {
                isTrigger = false;
                enemiesHit.Clear();
            }
    }

    public void AttackActivation()
    {
        isAttacking = true;
        isOnCooldown = true;
        isTrigger = true;
        playerWeaponSlot.EquippedWeapon.WeaponAttackFX(stats.GetModifiedMainStat(MainStat.AttackSpeed));
    }

    public void HitboxDetection()
    {
        if (playerWeaponSlot.EquippedWeapon.WeaponHitBoxResult(out RaycastHit2D[] collisionsList))
        {   
            foreach (RaycastHit2D collision in collisionsList)
            {
                //Cast to enemy script
                EnemyLifeSystem enemyLifesystem = collision.transform.GetComponent<EnemyLifeSystem>();

                //Check enemy then apply damages and add to list
                if (enemyLifesystem && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < playerWeaponSlot.EquippedWeapon.TargetMax)
                {
                    //Apply damages
                    enemyLifesystem.TakeDamage(stats.GetModifiedMainStat(MainStat.Damage));

                    //Bump enemy away from hit
                    collision.transform.GetComponent<EnemyBump>().BumpedAwayActivation(-collision.normal);

                    //Add enemy to list
                    enemiesHit.Add(enemyLifesystem);

                }

            }

        }

    }

}
