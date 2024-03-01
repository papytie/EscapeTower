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
    
    [Header("Debug")]
    [SerializeField] bool showDebug = false;
    [SerializeField] Color debugColor = Color.red;
    [SerializeField] Mesh debugCube;

    float coolDownTime = 0;
    bool isOnCooldown = false;
    float attackLagTime = 0; 
    bool isAttacking = false;
    bool isTrigger = false;
    float hitboxTime = 0;
    float hitboxTimerDurationTotal = 0;
    float hitboxTimerDurationTick = 0;
    int tickCount = 0;
    bool showHitboxInDebug = false;
    float showHitboxTime = 0;

    List<EnemyLifeSystem> enemiesHit = new();

    PlayerWeaponSlot weaponSlot;
    PlayerStats stats;

    float angle;
    Vector2 startPos;
    Vector2 endPos;
    Quaternion startRotation;

    public void InitRef(PlayerWeaponSlot weaponSlotRef, PlayerStats statsRef)
    {
        weaponSlot = weaponSlotRef;
        stats = statsRef;
    }

    void Update()
    {
        if(isTrigger)
        {
            switch (weaponSlot.EquippedWeapon.DetectionType)
            {
                case HitboxDetectionType.EachFrame:

                    HitboxDetection(Mathf.Clamp01(hitboxTime / hitboxTimerDurationTotal));

                    if (TimeUtils.CustomTimer(ref hitboxTime, hitboxTimerDurationTotal))
                    {
                        isTrigger = false;
                        enemiesHit.Clear();
                    }
                    break;

                //CustomTick divide HitboxDuration in X individual detection of 1 frame
                case HitboxDetectionType.CustomTick:
                    if(TimeUtils.CustomTimer(ref hitboxTime, hitboxTimerDurationTick))
                    {
                        HitboxDetection(Mathf.Clamp01(tickCount * hitboxTimerDurationTick / hitboxTimerDurationTotal));

                        showHitboxInDebug = true;
                        tickCount++;
                        if (tickCount >= weaponSlot.EquippedWeapon.HitboxTicks) 
                        {
                            tickCount = 0;
                            isTrigger = false;
                            enemiesHit.Clear();               
                        }
                    }
                    break;
            }
        }

        //------------------------------------------------Debug--------------------------------------------------
        if (showHitboxInDebug && TimeUtils.CustomTimer(ref showHitboxTime, .05f))
            showHitboxInDebug = false;
        //------------------------------------------------Debug--------------------------------------------------

        if (isOnCooldown && TimeUtils.CustomTimer(ref coolDownTime, stats.GetModifiedSecondaryStat(SecondaryStat.AttackCooldown)))
            isOnCooldown = false;
        
        if (isAttacking && TimeUtils.CustomTimer(ref attackLagTime, stats.GetModifiedSecondaryStat(SecondaryStat.AttackLag)))
            isAttacking = false;
    }

    public void AttackActivation()
    {
        hitboxTimerDurationTotal = stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration);
        switch (weaponSlot.EquippedWeapon.DetectionType)
        {
            case HitboxDetectionType.EachFrame:
                hitboxTime = 0;
                break;

            case HitboxDetectionType.CustomTick:
                hitboxTimerDurationTick = hitboxTimerDurationTotal / (weaponSlot.EquippedWeapon.HitboxTicks - 1);
                hitboxTime = hitboxTimerDurationTick;
                break;
        }

        /*//initialise all values for hitbox
        startPos = weaponSlot.EquippedWeapon.HitboxStartPos;
        endPos = weaponSlot.EquippedWeapon.HitboxTargetPos;
        angle = weaponSlot.EquippedWeapon.HitboxAngle;
        startRotation = weaponSlot.EquippedWeapon.transform.rotation;*/

        isAttacking = true;
        isOnCooldown = true;
        isTrigger = true;
        weaponSlot.EquippedWeapon.AttackFX(stats.GetModifiedMainStat(MainStat.AttackSpeed));

        if (weaponSlot.EquippedWeapon.IsSpawningProjectile)
            weaponSlot.EquippedWeapon.SpawnProjectile();
        
    }

    public void HitboxDetection(float currentTime)
    {
        if (weaponSlot.EquippedWeapon.HitBoxResult(currentTime, out RaycastHit2D[] collisionsList))
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

    private void OnDrawGizmos()
    {
        if(showDebug && Application.isPlaying && (weaponSlot.EquippedWeapon.DetectionType == HitboxDetectionType.CustomTick && showHitboxInDebug || weaponSlot.EquippedWeapon.DetectionType == HitboxDetectionType.EachFrame && isTrigger))
        {
            weaponSlot.EquippedWeapon.GetPositions(weaponSlot.EquippedWeapon.RelativeTo, out Vector2 startPos, out Vector2 targetPos, out Quaternion rotation);

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
