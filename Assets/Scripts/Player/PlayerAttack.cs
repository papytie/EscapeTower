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
    float hitboxTimerDuration = 0;
    int tickCount = 0;

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
        if (isOnCooldown && TimeUtils.CustomTimer(ref coolDownTime, stats.GetModifiedSecondaryStat(SecondaryStat.AttackCooldown)))
            isOnCooldown = false;
        
        if (isAttacking && TimeUtils.CustomTimer(ref attackLagTime, stats.GetModifiedSecondaryStat(SecondaryStat.AttackLag)))
            isAttacking = false;

        if (isTrigger)
        {
            if(weaponSlot.EquippedWeapon.DetectionType == HitboxDetectionType.EachFrame)
            {
                HitboxDetection();
            }

            if (TimeUtils.CustomTimer(ref hitboxTime, hitboxTimerDuration))
            {
                isTrigger = false;
                enemiesHit.Clear();
            }


        }
            
           /* switch (weaponSlot.EquippedWeapon.DetectionType)
            {
                case HitboxDetectionType.EachFrame:
                    HitboxDetection();
                    Debug.Log("Detection each frame");
                    if (isTrigger && TimeUtils.CustomTimer(ref hitboxTime, hitboxTimerDuration))
                    {
                        isTrigger = false;
                        enemiesHit.Clear();
                    }
                    break;

                case HitboxDetectionType.CustomTick:
                    if(isTrigger && TimeUtils.CustomTimer(ref hitboxTime, hitboxTimerDuration))
                    {
                        Debug.Log("Detection on tick");
                        Debug.Log("currentTime " + hitboxTime);
                        Debug.Log("duration " + hitboxTimerDuration);
                        HitboxDetection();
                        tickCount++;
                        if (tickCount >= weaponSlot.EquippedWeapon.HitboxTicks - 1) 
                        {
                            isTrigger = false;
                            enemiesHit.Clear();               
                        }
                    }
                    break;
            }*/
    }

    public void AttackActivation()
    {
        switch (weaponSlot.EquippedWeapon.DetectionType)
        {
            case HitboxDetectionType.EachFrame:
                hitboxTimerDuration = stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration);
                hitboxTime = 0;
                break;

            case HitboxDetectionType.CustomTick:
                hitboxTimerDuration = stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration) / (weaponSlot.EquippedWeapon.HitboxTicks - 1);
                hitboxTime = hitboxTimerDuration;
                break;
        }

        isAttacking = true;
        isOnCooldown = true;
        isTrigger = true;
        weaponSlot.EquippedWeapon.WeaponAttackFX(stats.GetModifiedMainStat(MainStat.AttackSpeed));
    }

    public void HitboxDetection()
    {
        if (weaponSlot.EquippedWeapon.WeaponHitBoxResult(out RaycastHit2D[] collisionsList, hitboxTime/hitboxTimerDuration))
        {   
            foreach (RaycastHit2D collision in collisionsList)
            {
                //Cast to enemy script
                EnemyLifeSystem enemyLifesystem = collision.transform.GetComponent<EnemyLifeSystem>();

                //Check enemy then apply damages and add to list
                if (enemyLifesystem && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < weaponSlot.EquippedWeapon.TargetMax)
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

    private void OnDrawGizmos()
    {
        if(showDebug && Application.isPlaying && isTrigger)
        {
            switch (weaponSlot.EquippedWeapon.ShapeType)
            {
                case HitboxShapeType.Circle:
                    Gizmos.color = debugColor;
                    Gizmos.DrawSphere(weaponSlot.EquippedWeapon.HitboxCurrentPos, weaponSlot.EquippedWeapon.CircleRadius);
                    Gizmos.color = Color.white;
                    break;

                case HitboxShapeType.Box:
                    Gizmos.color = debugColor;
                    Gizmos.DrawMesh(debugCube, -1, weaponSlot.EquippedWeapon.HitboxCurrentPos, weaponSlot.EquippedWeapon.transform.rotation, weaponSlot.EquippedWeapon.BoxSize);
                    Gizmos.color = Color.white;
                    break;

            }    
        }
    }
}
