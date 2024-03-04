using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAttack : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsAttacking => isAttacking;
    public bool IsOnAttackLag => isOnAttackLag;
    public bool AutoAttackOnStick => autoAttackOnStick;
    public float AttackSpeed => attackSpeed;

    [Header("Attack Settings")]
    [SerializeField] bool autoAttackOnStick = true;
    [SerializeField] float attackSpeed = 1;
    
    [Header("Debug")]
    [SerializeField] bool showDebug = false;
    [SerializeField] Color debugColor = Color.red;
    [SerializeField] Mesh debugCube;

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0; 
    bool isOnAttackLag = false;
    float hitboxEndTime = 0;
    bool isOnHitboxDetection = false;
    float showHitboxEndTime = 0;
    bool showHitboxInDebug = false;
    bool isAttacking = false;

    List<EnemyLifeSystem> enemiesHit = new();

    PlayerWeaponSlot weaponSlot;
    PlayerStats stats;

    public void InitRef(PlayerWeaponSlot weaponSlotRef, PlayerStats statsRef)
    {
        weaponSlot = weaponSlotRef;
        stats = statsRef;
    }

    void Update()
    {
        if (isOnAttackLag && Time.time >= attackLagEndTime)
            isOnAttackLag = false;

        if(isOnHitboxDetection)
        {
            HitboxDetection();
            if(Time.time >= hitboxEndTime)
            {
                isOnHitboxDetection = false;
                isAttacking = false;
                SetAttackCooldownTimer();
            }
        }

        if (isOnCooldown && Time.time >= cooldownEndTime)
            isOnCooldown = false;
        
        //------------------------------------------------Debug--------------------------------------------------
        if (showHitboxInDebug && Time.time >= showHitboxEndTime)
            showHitboxInDebug = false;
        //------------------------------------------------Debug--------------------------------------------------
    }

    public void AttackActivation()
    {
        isAttacking = true;
        SetAttackLagTimer();
        SetHitboxDetectionTimer();
        weaponSlot.EquippedWeapon.AttackFX(stats.GetModifiedMainStat(MainStat.AttackSpeed));
    }

    public void HitboxDetection()
    {
        if (weaponSlot.EquippedWeapon.HitBoxResult(out RaycastHit2D[] collisionsList))
        {   
            foreach (RaycastHit2D collision in collisionsList)
            {
                EnemyLifeSystem enemyLifesystem = collision.transform.GetComponent<EnemyLifeSystem>();

                if (enemyLifesystem && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < weaponSlot.EquippedWeapon.TargetMax)
                {
                    enemyLifesystem.TakeDamage(stats.GetModifiedMainStat(MainStat.Damage));

                    //Call enemy Bump and give direction which is the inverted Normal of the collision
                    collision.transform.GetComponent<EnemyBump>().BumpedAwayActivation(-collision.normal);

                    enemiesHit.Add(enemyLifesystem);

                }

            }

        }

    }

    void SetAttackCooldownTimer()
    {
        cooldownEndTime = Time.time + stats.GetModifiedSecondaryStat(SecondaryStat.AttackCooldownDuration);
        isOnCooldown = true;
    }

    void SetHitboxDetectionTimer()
    {
        hitboxEndTime = Time.time + stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration);
        isOnHitboxDetection = true;
    }

    void SetAttackLagTimer()
    {
        attackLagEndTime = Time.time + stats.GetModifiedSecondaryStat(SecondaryStat.AttackLagDuration);
        isOnAttackLag = true;
    }

    private void OnDrawGizmos()
    {
        if(showDebug && Application.isPlaying && (weaponSlot.EquippedWeapon.DetectionType == HitboxDetectionType.CustomTick && showHitboxInDebug || weaponSlot.EquippedWeapon.DetectionType == HitboxDetectionType.EachFrame && isOnHitboxDetection))
        {
            weaponSlot.EquippedWeapon.GetPositions(weaponSlot.EquippedWeapon.RelativeTo, out Vector2 startPos, out Quaternion rotation);

            switch (weaponSlot.EquippedWeapon.ShapeType)
            {
                case HitboxShapeType.Circle:
                    Gizmos.color = debugColor;
                    Gizmos.DrawSphere(weaponSlot.EquippedWeapon.HitboxCurrentPos, weaponSlot.EquippedWeapon.CircleRadius);
                    Gizmos.color = Color.white;
                    break;

                case HitboxShapeType.Box:
                    Gizmos.color = debugColor;
                    Gizmos.DrawMesh(debugCube, -1, weaponSlot.EquippedWeapon.HitboxCurrentPos, rotation, weaponSlot.EquippedWeapon.BoxSize);
                    Gizmos.color = Color.white;
                    break;

            }    
        }
    }
}
