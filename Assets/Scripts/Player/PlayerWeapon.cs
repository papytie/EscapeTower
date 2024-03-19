using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PlayerWeapon : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsOnAttackLag => isOnAttackLag;
    public AttackData WeaponData => weaponData;

    Animator weaponAnimator;
    PlayerStats stats;
    PlayerWeaponSlot weaponSlot;
    [SerializeField] AttackData weaponData;

    List<EnemyLifeSystem> enemiesHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;

    public Transform HitboxRelativeTransform => GetRelativeTransform(weaponData.hitboxPositionRelativeTo);
    public Vector3 HitboxStartPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(weaponData.hitboxPositionOffset);
    public Vector3 HitboxTargetPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(weaponData.targetPosition);
    public Transform SpawnRelativeTransform => GetRelativeTransform(weaponData.projectileSpawnRelativeTo);
    public Vector3 ProjectileSpawnPosition => SpawnRelativeTransform.position + SpawnRelativeTransform.TransformVector(weaponData.projectileSpawnOffset);


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
        yield return new WaitForSeconds(weaponData.delay);

        if (weaponData.useMeleeHitbox)
            StartMeleeHitboxCheck();

        if (weaponData.useProjectile)
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

        if (weaponData.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));

            switch (weaponData.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, weaponData.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                    Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                    float angleValue = Vector2.Angle(startVector, endVector);
                    //TODO: when angle value is negative the vector didnt rotate in the good direction
                    Vector3 currentVector = Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, weaponData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector;
                    hitboxCurrrentPos = HitboxRelativeTransform.position + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, weaponData.hitboxMovementCurve.Evaluate(t));
                    break;
            }

        }

        collisionsList = weaponData.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrrentPos, weaponData.circleRadius, Vector2.zero, 0, weaponData.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrrentPos, weaponData.boxSize, Quaternion.Angle(Quaternion.identity, HitboxRelativeTransform.transform.rotation), Vector2.zero, 0, weaponData.targetLayer),
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

                if (enemyLifesystem && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < weaponData.maxTargets)
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

        if (weaponData.projectileNumber > 1)
        {
            float minAngle = weaponData.spreadAngle / 2f;
            float angleIncrValue = weaponData.spreadAngle / (weaponData.projectileNumber -1);

            for (int i = 0; i < weaponData.projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + weaponData.projectileAngleOffset, transform.forward);
                
                Instantiate(weaponData.projectileToSpawn, ProjectileSpawnPosition, SpawnRelativeTransform.rotation * angleResult)
                    .Init(gameObject, weaponData, ProjectileSpawnPosition, stats.GetModifiedMainStat(MainStat.Damage));

                if(weaponData.projectileSpawnType == ProjectileSpawnType.Sequence)
                {
                    float t = weaponData.duration / (weaponData.projectileNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(weaponData.projectileToSpawn, ProjectileSpawnPosition, SpawnRelativeTransform.rotation * Quaternion.AngleAxis(weaponData.projectileAngleOffset,transform.forward))
                .Init(gameObject, weaponData, ProjectileSpawnPosition, stats.GetModifiedMainStat(MainStat.Damage));
    }

    public void ChangeProjectile(ProjectileController newProjectile)
    {
        weaponData.projectileToSpawn = newProjectile;
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
        if (weaponData.showDebug)
        {
            Gizmos.color = weaponData.meleeDebugColor;
            if (weaponData.useMeleeHitbox)
            {

                switch (weaponData.hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(HitboxStartPosition, weaponData.circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(weaponData.debugCube, -1, HitboxStartPosition, HitboxRelativeTransform.rotation, weaponData.boxSize);
                        break;

                }

                if (weaponData.behaviorType != HitboxBehaviorType.Fixed)
                {
                    switch (weaponData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(HitboxTargetPosition, weaponData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(weaponData.debugCube, -1, HitboxTargetPosition, HitboxRelativeTransform.rotation, weaponData.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector3 hitboxCurrentPos = HitboxStartPosition;
                    float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));
                    switch (weaponData.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, weaponData.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                            Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                            float angleValue = Vector2.Angle(startVector, endVector);
                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, weaponData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                            hitboxCurrentPos = HitboxRelativeTransform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, weaponData.hitboxMovementCurve.Evaluate(t));
                            break;
                    }

                    switch (weaponData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, weaponData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(weaponData.debugCube, -1, hitboxCurrentPos, HitboxRelativeTransform.rotation, weaponData.boxSize);
                            break;
                    }
                }
            }
            Gizmos.color = Color.white;

            Gizmos.color = weaponData.projectileDebugColor;
            if (weaponData.useProjectile)
            {
                if (weaponData.projectileNumber > 1)
                {
                    float minAngle = weaponData.spreadAngle / 2f;
                    float angleIncrValue = weaponData.spreadAngle / (weaponData.projectileNumber - 1);

                    for (int i = 0; i < weaponData.projectileNumber; i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleResult = Quaternion.AngleAxis(angle + weaponData.projectileAngleOffset, transform.forward);

                        Vector3 multProjPos = ProjectileSpawnPosition + SpawnRelativeTransform.rotation * angleResult * Vector3.up * weaponData.projectileRange;
                        Vector3 multProjHitboxCurrentPos = multProjPos + SpawnRelativeTransform.TransformVector(angleResult * weaponData.projectileToSpawn.HitboxOffset);

                        switch (weaponData.projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawSphere(multProjHitboxCurrentPos, weaponData.projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawMesh(weaponData.debugCube, -1, multProjHitboxCurrentPos, SpawnRelativeTransform.rotation * angleResult, weaponData.projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(ProjectileSpawnPosition, multProjPos);
                    }
                }

                else
                {
                    Vector3 singleProjPos = ProjectileSpawnPosition + SpawnRelativeTransform.TransformVector(Quaternion.AngleAxis(weaponData.projectileAngleOffset, transform.forward) * Vector3.up * weaponData.projectileRange);
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + SpawnRelativeTransform.TransformVector(weaponData.projectileToSpawn.HitboxOffset);

                    switch (weaponData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(singleProjHitboxCurrentPos, weaponData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(weaponData.debugCube, -1, singleProjHitboxCurrentPos, SpawnRelativeTransform.rotation * Quaternion.AngleAxis(weaponData.projectileAngleOffset, transform.forward), weaponData.projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(ProjectileSpawnPosition, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }

}