using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class WeaponController : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsOnAttackLag => isOnAttackLag;
    public WeaponAttackData AttackData => weaponAttackData.attackData;
    public Transform HitboxRelativeTransform => GetRelativeTransform(AttackData.hitboxPositionRelativeTo);
    public Vector3 HitboxStartPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(AttackData.hitboxPositionOffset);
    public Vector3 HitboxTargetPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(AttackData.targetPosition);
    public Vector3 ProjectileSpawnPosition => transform.position + transform.TransformVector(AttackData.projectileData.spawnOffset);

    [Header("Ref"), Space]
    [SerializeField] PlayerWeaponSlot weaponSlot;
    [SerializeField] WeaponAttackConfig weaponAttackData;

    [Header("Debug"), Space]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color meleeDebugColor;
    [SerializeField] Color projectileDebugColor;

    Animator weaponAnimator;
    PlayerStats stats;
    List<EnemyLifeSystemComponent> enemiesHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
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

        if (meleeHitboxActive)
        {
            MeleeHitProcess();
            if (Time.time >= startTime + stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration))
            {
                meleeHitboxActive = false;
                isAttacking = false;
                enemiesHit.Clear();
                StartAttackCooldown();
            }
        }

        if (isOnCooldown && Time.time >= cooldownEndTime)
            isOnCooldown = false;
    }

    public void AttackActivation()
    {
        isAttacking = true;
        StartAttackLag();
        TriggerAttackAnim(stats.GetModifiedMainStat(MainStat.AttackSpeed));

        StartCoroutine(AttackProcess());
    }

    IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(AttackData.delay);

        if (AttackData.useMeleeHitbox)
            StartMeleeHitboxCheck();

        if (AttackData.useProjectile)
        {
            StartCoroutine(FireProjectile());
            StartAttackCooldown();
            isAttacking = false;
        }

    }

    void TriggerAttackAnim(float playerAttackSpeed)
    {
        weaponAnimator.SetFloat(SRAnimators.WeaponBaseAnim.Parameters.attackSpeed, playerAttackSpeed);
        weaponAnimator.SetTrigger(SRAnimators.WeaponBaseAnim.Parameters.attack);
    }

    bool WeaponHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector2 hitboxCurrrentPos = HitboxStartPosition;

        if (AttackData.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));

            switch (AttackData.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, AttackData.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                    Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                    float angleValue = Vector2.Angle(startVector, endVector);
                    
                    if (AttackData.targetPosition.x > AttackData.hitboxPositionOffset.x)
                        angleValue *= -1;

                    Vector3 currentVector = Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, AttackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector;

                    hitboxCurrrentPos = HitboxRelativeTransform.position + 
                        currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, AttackData.hitboxMovementCurve.Evaluate(t));

                    break;
            }

        }

        collisionsList = AttackData.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrrentPos, AttackData.circleRadius, Vector2.zero, 0, AttackData.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrrentPos, AttackData.boxSize, 
                Quaternion.Angle(Quaternion.identity, HitboxRelativeTransform.transform.rotation), Vector2.zero, 0, AttackData.targetLayer),
            _ => null,
        };
        return collisionsList.Length > 0;
    }

    void MeleeHitProcess()
    {
        if (WeaponHitboxCast(out RaycastHit2D[] collisionsList))
        {
            foreach (RaycastHit2D collision in collisionsList)
            {
                if (collision.transform.TryGetComponent(out EnemyLifeSystemComponent enemyLifeSystem))
                {
                    if(!enemyLifeSystem.IsInvincible && !enemiesHit.Contains(enemyLifeSystem) && enemiesHit.Count < AttackData.maxTargets)
                    {
                        enemyLifeSystem.TakeDamage(stats.GetModifiedMainStat(MainStat.Damage), collision.normal);
                        enemiesHit.Add(enemyLifeSystem);
                    }
                }
                if (collision.transform.TryGetComponent(out ProjectileController projectileController))
                {
                    if(projectileController.IsDestructible)
                        projectileController.DestroyOnAttack();
                }
            }
        }
    }

    IEnumerator FireProjectile()
    {
        //Player Projectiles are broken for now, they need a target
        if (stats.GetModifiedMainStat(MainStat.ProjectileNumber) > 1)
        {
            float minAngle = AttackData.projectileData.spreadAngle / 2f;
            float angleIncrValue = AttackData.projectileData.spreadAngle / (stats.GetModifiedMainStat(MainStat.ProjectileNumber) - 1);

            for (int i = 0; i < stats.GetModifiedMainStat(MainStat.ProjectileNumber); i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + AttackData.projectileData.angleOffset, transform.forward);
                
                Instantiate(AttackData.projectileData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * angleResult)
                    .Init(weaponSlot.gameObject, AttackData.projectileData, ProjectileSpawnPosition, stats.GetModifiedMainStat(MainStat.Damage), stats.GetModifiedMainStat(MainStat.ProjectileRange));

                if(AttackData.projectileData.spawnType == ProjectileSpawnType.Sequence)
                {
                    float t = AttackData.duration / (stats.GetModifiedMainStat(MainStat.ProjectileNumber) - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(AttackData.projectileData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * Quaternion.AngleAxis(AttackData.projectileData.angleOffset,transform.forward))
                .Init(weaponSlot.gameObject, AttackData.projectileData, ProjectileSpawnPosition, stats.GetModifiedMainStat(MainStat.Damage), stats.GetModifiedMainStat(MainStat.ProjectileRange));
    }

    void StartAttackCooldown()
    {
        cooldownEndTime = Time.time + stats.GetModifiedSecondaryStat(SecondaryStat.AttackCooldownDuration);
        isOnCooldown = true;
    }

    void StartMeleeHitboxCheck()
    {
        startTime = Time.time;
        meleeHitboxActive = true;
    }

    void StartAttackLag()
    {
        attackLagEndTime = Time.time + stats.GetModifiedSecondaryStat(SecondaryStat.AttackLagDuration);
        isOnAttackLag = true;
    }

    public Transform GetRelativeTransform(RelativeTransform relativeTransform)
    {
        return relativeTransform switch
        {
            RelativeTransform.ToWeapon => gameObject.transform,
            RelativeTransform.ToPlayer => weaponSlot.transform,
            RelativeTransform.ToWeaponSlot => weaponSlot.SlotTransform,
            _ => null,
        };
    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = meleeDebugColor;
            if (AttackData.useMeleeHitbox)
            {

                switch (AttackData.hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(HitboxStartPosition, AttackData.circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(debugCube, -1, HitboxStartPosition, HitboxRelativeTransform.rotation, AttackData.boxSize);
                        break;

                }

                if (AttackData.behaviorType != HitboxBehaviorType.Fixed)
                {
                    switch (AttackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(HitboxTargetPosition, AttackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, HitboxTargetPosition, HitboxRelativeTransform.rotation, AttackData.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector3 hitboxCurrentPos = HitboxStartPosition;
                    float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));
                    switch (AttackData.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, AttackData.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                            Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                            float angleValue = Vector2.Angle(startVector, endVector);

                            if(AttackData.targetPosition.x > AttackData.hitboxPositionOffset.x)
                                angleValue *= -1;

                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, AttackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);

                            hitboxCurrentPos = HitboxRelativeTransform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, AttackData.hitboxMovementCurve.Evaluate(t));
                            break;
                    }

                    switch (AttackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, AttackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, hitboxCurrentPos, HitboxRelativeTransform.rotation, AttackData.boxSize);
                            break;
                    }
                }
            }
            Gizmos.color = Color.white;

            Gizmos.color = projectileDebugColor;
            if (AttackData.useProjectile)
            {
                if (stats.GetModifiedMainStat(MainStat.ProjectileNumber) > 1)
                {
                    float minAngle = AttackData.projectileData.spreadAngle / 2f;
                    float angleIncrValue = AttackData.projectileData.spreadAngle / (stats.GetModifiedMainStat(MainStat.ProjectileNumber) - 1);

                    for (int i = 0; i < stats.GetModifiedMainStat(MainStat.ProjectileNumber); i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleRotation = Quaternion.AngleAxis(angle + AttackData.projectileData.angleOffset, transform.forward);

                        Vector3 multProjPos = ProjectileSpawnPosition + transform.rotation * angleRotation * Vector3.up * stats.GetModifiedMainStat(MainStat.ProjectileRange);
                        Vector3 multProjHitboxEndPos = multProjPos + transform.rotation * angleRotation * AttackData.projectileData.projectileToSpawn.HitboxOffset;

                        switch (AttackData.projectileData.projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawWireSphere(multProjHitboxEndPos, AttackData.projectileData.projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawWireMesh(debugCube, -1, multProjHitboxEndPos, transform.rotation * angleRotation, AttackData.projectileData.projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(ProjectileSpawnPosition, multProjPos);
                    }
                }

                else
                {
                    Vector3 singleProjPos = ProjectileSpawnPosition + transform.TransformVector(Quaternion.AngleAxis(AttackData.projectileData.angleOffset, transform.forward) * Vector3.up * stats.GetModifiedMainStat(MainStat.ProjectileRange));
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + transform.TransformVector(AttackData.projectileData.projectileToSpawn.HitboxOffset);

                    switch (AttackData.projectileData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(singleProjHitboxCurrentPos, AttackData.projectileData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, singleProjHitboxCurrentPos, transform.rotation * Quaternion.AngleAxis(AttackData.projectileData.angleOffset, transform.forward), AttackData.projectileData.projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(ProjectileSpawnPosition, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }

}