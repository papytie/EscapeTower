using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PlayerWeapon : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsOnAttackLag => isOnAttackLag;
    public bool ShowDebug => showDebug;
    public bool ProjectileReturnFlip => projectileReturnFlip;
    public float Damage => damage;
    public float Cooldown => cooldown;
    public float Lag => lag;
    public float HitboxDuration => duration;
    public float Range => projectileRange;
    public float Speed => projectileSpeed;
    public float MaxTargets => projectileMaxTargets;
    public Color DebugColor => meleeDebugColor;
    public AnimationCurve LaunchCurve => launchCurve;
    public AnimationCurve ReturnCurve => returnCurve;
    public ProjectileReturnType ProjectileReturnType => projectileReturnType;

    [Header("Weapon Settings")]
    [SerializeField] PlayerWeaponSlot weaponSlot;
    [SerializeField] float damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;

    [Header("Attack Settings")]
    [SerializeField] float delay = 0;
    [SerializeField] float duration = .1f;
    [SerializeField] LayerMask enemyLayer;

    [Header("Melee Settings")]
    [SerializeField] bool useMeleeHitbox = true;
    [SerializeField] int maxTargets = 1;
    [SerializeField] RelativeTransform hitboxPositionRelativeTo;
    [SerializeField] Vector2 hitboxPositionOffset = Vector2.zero;    
    [SerializeField] HitboxShapeType hitboxShape;
    [SerializeField] float circleRadius = .1f;
    [SerializeField] Vector2 boxSize = new(.1f, .1f);
    [SerializeField] HitboxBehaviorType behaviorType;
    [SerializeField] AnimationCurve hitboxMovementCurve;
    [SerializeField] Vector2 targetPosition = Vector2.zero;

    [Header("Projectile Option")]
    [SerializeField] bool useProjectile = false;
    [SerializeField] AnimationCurve launchCurve;
    [SerializeField] ProjectileReturnType projectileReturnType;
    [SerializeField] bool projectileReturnFlip = false;
    [SerializeField] AnimationCurve returnCurve;
    [SerializeField] PlayerProjectile projectileToSpawn;
    [SerializeField] RelativeTransform projectileSpawnRelativeTo;
    [SerializeField] Vector2 projectileSpawnOffset = Vector2.zero;    
    [SerializeField] float projectileAngleOffset = 0;
    [SerializeField] int projectileNumber = 1;
    [SerializeField] ProjectileSpawnType projectileSpawnType;
    [SerializeField] int projectileMaxTargets = 1;
    [SerializeField] float projectileSpeed = 1;
    [SerializeField] float spreadAngle = 60;
    [SerializeField] float projectileRange = 1;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color meleeDebugColor;
    [SerializeField] Color projectileDebugColor;

    Animator weaponAnimator;
    PlayerStats stats;

    List<EnemyLifeSystem> enemiesHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;

    public Transform HitboxRelativeTransform => GetRelativeTransform(hitboxPositionRelativeTo);
    public Vector3 HitboxStartPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(hitboxPositionOffset);
    public Vector3 HitboxTargetPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(targetPosition);
    public Transform SpawnRelativeTransform => GetRelativeTransform(projectileSpawnRelativeTo);
    public Vector3 ProjectileSpawnPosition => SpawnRelativeTransform.position + SpawnRelativeTransform.TransformVector(projectileSpawnOffset);


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
        yield return new WaitForSeconds(delay);

        if (useMeleeHitbox)
            StartMeleeHitboxCheck();

        if (useProjectile)
        {
            StartCoroutine(FireProjectile());
            StartAttackCooldown();
            isAttacking = false;
        }

    }

    void TriggerAttackAnim(float playerAttackSpeed)
    {
        weaponAnimator.SetTrigger(GameParams.Animation.WEAPON_ATTACK_TRIGGER);
        weaponAnimator.SetFloat(GameParams.Animation.WEAPON_ATTACKSPEED_FLOAT, playerAttackSpeed);
    }

    bool WeaponHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector2 hitboxCurrrentPos = HitboxStartPosition;

        if (behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));

            switch (behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                    Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                    float angleValue = Vector2.Angle(startVector, endVector);
                    Vector3 currentVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                    hitboxCurrrentPos = HitboxRelativeTransform.position + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, hitboxMovementCurve.Evaluate(t));
                    break;
            }

        }

        collisionsList = hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrrentPos, circleRadius, Vector2.zero, 0, enemyLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrrentPos, boxSize, Quaternion.Angle(Quaternion.identity, HitboxRelativeTransform.transform.rotation), Vector2.zero, 0, enemyLayer),
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
                EnemyLifeSystem enemyLifesystem = collision.transform.GetComponent<EnemyLifeSystem>();

                if (enemyLifesystem && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < maxTargets)
                {
                    enemyLifesystem.TakeDamage(stats.GetModifiedMainStat(MainStat.Damage));

                    //Call enemy Bump and give direction which is the inverted Normal of the collision
                    collision.transform.GetComponent<EnemyBump>().BumpedAwayActivation(-collision.normal);

                    enemiesHit.Add(enemyLifesystem);
                }
            }
        }
    }

    IEnumerator FireProjectile()
    {

        if (projectileNumber > 1)
        {
            float minAngle = spreadAngle / 2f;
            float angleIncrValue = spreadAngle / (projectileNumber-1);

            for (int i = 0; i < projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + projectileAngleOffset, transform.forward);
                
                Instantiate(projectileToSpawn, ProjectileSpawnPosition, SpawnRelativeTransform.rotation * angleResult).Init(this, stats, ProjectileSpawnPosition, projectileRange);

                if(projectileSpawnType == ProjectileSpawnType.Sequence)
                {
                    float t = duration / (projectileNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(projectileToSpawn, ProjectileSpawnPosition, SpawnRelativeTransform.rotation * Quaternion.AngleAxis(projectileAngleOffset,transform.forward)).Init(this, stats, ProjectileSpawnPosition, projectileRange);
    }

    public void ChangeProjectile(PlayerProjectile newProjectile)
    {
        projectileToSpawn = newProjectile;
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

    public Transform GetRelativeTransform(RelativeTransform transform)
    {
        return transform switch
        {
            RelativeTransform.ToWeapon => base.transform,
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
            if (useMeleeHitbox)
            {

                switch (hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(HitboxStartPosition, circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(debugCube, -1, HitboxStartPosition, HitboxRelativeTransform.rotation, boxSize);
                        break;

                }

                if (behaviorType != HitboxBehaviorType.Fixed)
                {
                    switch (hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(HitboxTargetPosition, circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, HitboxTargetPosition, HitboxRelativeTransform.rotation, boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector3 hitboxCurrentPos = HitboxStartPosition;
                    float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));
                    switch (behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                            Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                            float angleValue = Vector2.Angle(startVector, endVector);
                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                            hitboxCurrentPos = HitboxRelativeTransform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, hitboxMovementCurve.Evaluate(t));
                            break;
                    }

                    switch (hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, hitboxCurrentPos, HitboxRelativeTransform.rotation, boxSize);
                            break;
                    }
                }
            }
            Gizmos.color = Color.white;

            Gizmos.color = projectileDebugColor;
            if (useProjectile)
            {

                if (projectileNumber > 1)
                {
                    float minAngle = spreadAngle / 2f;
                    float angleIncrValue = spreadAngle / (projectileNumber - 1);

                    for (int i = 0; i < projectileNumber; i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleResult = Quaternion.AngleAxis(angle + projectileAngleOffset, transform.forward);

                        Vector3 multProjPos = ProjectileSpawnPosition + SpawnRelativeTransform.TransformVector(angleResult * Vector3.up * projectileRange);
                        Vector3 multProjHitboxCurrentPos = multProjPos + SpawnRelativeTransform.TransformVector(angleResult * projectileToSpawn.HitboxOffset);

                        switch (projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawSphere(multProjHitboxCurrentPos, projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawMesh(debugCube, -1, multProjHitboxCurrentPos, SpawnRelativeTransform.rotation * angleResult, projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(ProjectileSpawnPosition, multProjPos);
                    }
                }

                else
                {
                    Vector3 singleProjPos = ProjectileSpawnPosition + SpawnRelativeTransform.TransformVector(Quaternion.AngleAxis(projectileAngleOffset, transform.forward) * Vector3.up * projectileRange);
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + SpawnRelativeTransform.TransformVector(projectileToSpawn.HitboxOffset);

                    switch (projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(singleProjHitboxCurrentPos, projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, singleProjHitboxCurrentPos, SpawnRelativeTransform.rotation * Quaternion.AngleAxis(projectileAngleOffset, transform.forward), projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(ProjectileSpawnPosition, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }

}