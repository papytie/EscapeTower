using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxShapeType
{
    Box = 1,
    Circle = 2
}

public enum HitboxRelativePosition
{
    ToWeapon = 0,
    ToPlayer = 1,
    ToWeaponSlot = 2,
}

public enum WeaponAttackType
{
    Melee = 0,
    Ranged = 1,
}

[RequireComponent(typeof(Animator))]

public class PlayerWeapon : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsOnAttackLag => isOnAttackLag;
    public bool ShowDebug => showDebug;
    public float Damage => damage;
    public float Cooldown => cooldown;
    public float Lag => lag;
    public float HitboxDuration => duration;
    public float Range => range;
    public Color DebugColor => debugColor;

    [Header("Weapon Settings")]
    [SerializeField] WeaponAttackType attackType;
    [SerializeField] float damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;
    [Header("Projectile Option")]
    [SerializeField] PlayerProjectile projectileToSpawn;
    [SerializeField] float range = 1;
    [SerializeField] int projectileNumber = 1;
    [SerializeField] float spreadAngle = 60;

    [Header("Hitbox Settings")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] HitboxRelativePosition relativePostition;
    [SerializeField] Vector2 hitboxPositionOffset = Vector2.zero;
    [SerializeField] bool isMoving;
    [SerializeField] Vector2 targetPosition = Vector2.zero;
    [SerializeField] float duration = .1f;
    [SerializeField] int numberOfTarget = 1;

    [Header("Hitbox Shape")]
    [SerializeField] HitboxShapeType shapeType;
    [SerializeField] float circleRadius = .1f;
    [SerializeField] Vector2 boxSize = new(.1f, .1f);

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color debugColor;

    Animator weaponAnimator;
    PlayerWeaponSlot weaponSlot;
    PlayerStats stats;

    List<EnemyLifeSystem> enemiesHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool isOnHitboxDetection = false;
    bool isAttacking = false;

    private void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    public void InitRef(PlayerWeaponSlot slotRef, PlayerStats statsRef)
    {
        weaponSlot = slotRef;
        stats = statsRef;
    }

    private void Update()
    {
        if (isOnAttackLag && Time.time >= attackLagEndTime)
            isOnAttackLag = false;

        if (isOnHitboxDetection)
        {
            MeleeHitboxDetection();
            if (Time.time >= startTime + stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration))
            {
                isOnHitboxDetection = false;
                isAttacking = false;
                enemiesHit.Clear();
                SetAttackCooldownTimer();
            }
        }

        if (isOnCooldown && Time.time >= cooldownEndTime)
            isOnCooldown = false;

    }

    public void AttackActivation()
    {
        isAttacking = true;
        SetAttackLagTimer();
        TriggerAttackAnim(stats.GetModifiedMainStat(MainStat.AttackSpeed));

        switch (attackType)
        {
            case WeaponAttackType.Melee:
                SetMeleeHitboxDetectionTimer();
                break;

            case WeaponAttackType.Ranged:
                SpawnProjectile();
                SetAttackCooldownTimer();
                isAttacking = false;
                break;

            default:
                Debug.LogWarning("Weapon Attack Type is not defined");
                break;
        }
    }

    void TriggerAttackAnim(float playerAttackSpeed)
    {
        weaponAnimator.SetTrigger(GameParams.Animation.WEAPON_ATTACK_TRIGGER);
        weaponAnimator.SetFloat(GameParams.Animation.WEAPON_ATTACKSPEED_FLOAT, playerAttackSpeed);
    }

    bool WeaponHitBoxResult(out RaycastHit2D[] collisionsList)
    {
        Transform relative = GetRelativeTransform(relativePostition);
        Vector3 offsetPos = relative.position + (relative.right * hitboxPositionOffset.x + relative.up * hitboxPositionOffset.y);

        if (isMoving)
        {
            Vector2 targetPos = relative.position + (relative.right * targetPosition.x + relative.up * targetPosition.y);
            float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));
            offsetPos = Vector2.Lerp(offsetPos, targetPos, t);
        }

        collisionsList = shapeType switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(offsetPos, circleRadius, Vector2.zero, 0, enemyLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(offsetPos, boxSize, Quaternion.Angle(Quaternion.identity, relative.transform.rotation), Vector2.zero, 0, enemyLayer),
            _ => null,
        };
        return collisionsList.Length > 0;
    }

    void MeleeHitboxDetection()
    {
        if (WeaponHitBoxResult(out RaycastHit2D[] collisionsList))
        {
            foreach (RaycastHit2D collision in collisionsList)
            {
                EnemyLifeSystem enemyLifesystem = collision.transform.GetComponent<EnemyLifeSystem>();

                if (enemyLifesystem && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < numberOfTarget)
                {
                    enemyLifesystem.TakeDamage(stats.GetModifiedMainStat(MainStat.Damage));

                    //Call enemy Bump and give direction which is the inverted Normal of the collision
                    collision.transform.GetComponent<EnemyBump>().BumpedAwayActivation(-collision.normal);

                    enemiesHit.Add(enemyLifesystem);
                }
            }
        }
    }

    void SpawnProjectile()
    {
        Transform relative = GetRelativeTransform(relativePostition);
        Vector3 offsetPos = relative.position + relative.rotation * hitboxPositionOffset;
        if (projectileNumber > 1)
        {
            float minAngle = spreadAngle / 2f;
            float angleIncrValue = spreadAngle / (projectileNumber-1);

            for (int i = 0; i < projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle, transform.forward);
                
                Instantiate(projectileToSpawn, offsetPos, relative.rotation * angleResult).Init(this, stats, relative.position, range);
            }
        }
        else Instantiate(projectileToSpawn, offsetPos, relative.rotation).Init(this, stats, relative.position, range);
    }

    void SetAttackCooldownTimer()
    {
        cooldownEndTime = Time.time + stats.GetModifiedSecondaryStat(SecondaryStat.AttackCooldownDuration);
        isOnCooldown = true;
    }

    void SetMeleeHitboxDetectionTimer()
    {
        startTime = Time.time;
        isOnHitboxDetection = true;
    }

    void SetAttackLagTimer()
    {
        attackLagEndTime = Time.time + stats.GetModifiedSecondaryStat(SecondaryStat.AttackLagDuration);
        isOnAttackLag = true;
    }

    public Transform GetRelativeTransform(HitboxRelativePosition relativeTo)
    {
        return relativeTo switch
        {
            HitboxRelativePosition.ToWeapon => transform,
            HitboxRelativePosition.ToPlayer => weaponSlot.transform,
            HitboxRelativePosition.ToWeaponSlot => weaponSlot.SlotTransform,
            _ => null,
        };
    }


    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Transform relative = GetRelativeTransform(relativePostition);
            Vector3 offsetPos = relative.position + relative.rotation * hitboxPositionOffset;

            Gizmos.color = debugColor;
            switch (attackType)
            {
                case WeaponAttackType.Melee:

                    switch (shapeType)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(offsetPos, circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, offsetPos, relative.rotation, boxSize);
                            break;

                    }

                    if(isMoving)
                    {
                        Vector2 targetPos = relative.position + (relative.right * targetPosition.x + relative.up * targetPosition.y);

                        switch (shapeType)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawWireSphere(targetPos, circleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawWireMesh(debugCube, -1, targetPos, relative.rotation, boxSize);
                                break;

                        }
                        float t = Mathf.Clamp01((Time.time - startTime) / duration);
                        offsetPos = Vector2.Lerp(offsetPos, targetPos, t);
                    }

                    if (isOnHitboxDetection)
                    {
                        switch (shapeType)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawSphere(offsetPos, circleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawMesh(debugCube, -1, offsetPos, relative.rotation, boxSize);
                                break;
                        }
                    }
                    break;

                case WeaponAttackType.Ranged:
                    if (projectileNumber > 1)
                    {
                        float minAngle = spreadAngle / 2f;
                        float angleIncrValue = spreadAngle / (projectileNumber - 1);

                        for (int i = 0; i < projectileNumber; i++)
                        {
                            float angle = minAngle - i * angleIncrValue;
                            Quaternion angleResult = Quaternion.AngleAxis(angle, transform.forward);

                            Vector3 multProjDebugPos = offsetPos + (relative.rotation * angleResult * Vector3.up) * range;
                            Vector3 hitboxOffsetPos = multProjDebugPos + relative.rotation * angleResult * projectileToSpawn.HitboxOffset;

                            switch (projectileToSpawn.HitboxShape)
                            {
                                case HitboxShapeType.Circle:
                                    Gizmos.DrawSphere(hitboxOffsetPos, circleRadius);
                                    break;

                                case HitboxShapeType.Box:
                                    Gizmos.DrawMesh(debugCube, -1, hitboxOffsetPos, relative.rotation * angleResult, projectileToSpawn.BoxSize);
                                    break;

                            }
                            Gizmos.DrawLine(offsetPos, multProjDebugPos);
                        }
                    }

                    else
                    {
                        Vector3 simpleProjDebugPos = offsetPos + (relative.rotation * Vector3.up) * range;
                        Vector3 hitboxOffsetPos = simpleProjDebugPos + relative.rotation * projectileToSpawn.HitboxOffset;

                        switch (projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawSphere(hitboxOffsetPos, circleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawMesh(debugCube, -1, hitboxOffsetPos, relative.rotation, projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(offsetPos, simpleProjDebugPos);

                    }
                    break;
            }
            Gizmos.color = Color.white;

        }
        
        


    }
}