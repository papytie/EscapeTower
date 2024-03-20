using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PlayerWeapon : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsOnAttackLag => isOnAttackLag;
    public AttackData WeaponData => attackData;

    Animator weaponAnimator;
    PlayerStats stats;
    PlayerWeaponSlot weaponSlot;
    [SerializeField] AttackData attackData;

    List<EnemyLifeSystem> enemiesHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;

    public Transform HitboxRelativeTransform => GetRelativeTransform(attackData.hitboxPositionRelativeTo);
    public Vector3 HitboxStartPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(attackData.hitboxPositionOffset);
    public Vector3 HitboxTargetPosition => HitboxRelativeTransform.position + HitboxRelativeTransform.TransformVector(attackData.targetPosition);
    public Transform SpawnRelativeTransform => GetRelativeTransform(attackData.projectileSpawnRelativeTo);
    public Vector3 ProjectileSpawnPosition => SpawnRelativeTransform.position + SpawnRelativeTransform.TransformVector(attackData.projectileSpawnOffset);


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
        yield return new WaitForSeconds(attackData.delay);

        if (attackData.useMeleeHitbox)
            StartMeleeHitboxCheck();

        if (attackData.useProjectile)
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

        if (attackData.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));

            switch (attackData.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, attackData.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                    Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                    float angleValue = Vector2.Angle(startVector, endVector);
                    //TODO: when angle value is negative the vector didnt rotate in the good direction
                    Vector3 currentVector = Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, attackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector;
                    hitboxCurrrentPos = HitboxRelativeTransform.position + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, attackData.hitboxMovementCurve.Evaluate(t));
                    break;
            }

        }

        collisionsList = attackData.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrrentPos, attackData.circleRadius, Vector2.zero, 0, attackData.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrrentPos, attackData.boxSize, Quaternion.Angle(Quaternion.identity, HitboxRelativeTransform.transform.rotation), Vector2.zero, 0, attackData.targetLayer),
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

                if (enemyLifesystem && !enemiesHit.Contains(enemyLifesystem) && enemiesHit.Count < attackData.maxTargets)
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

        if (attackData.projectileNumber > 1)
        {
            float minAngle = attackData.spreadAngle / 2f;
            float angleIncrValue = attackData.spreadAngle / (attackData.projectileNumber -1);

            for (int i = 0; i < attackData.projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + attackData.projectileAngleOffset, transform.forward);
                
                Instantiate(attackData.projectileToSpawn, ProjectileSpawnPosition, SpawnRelativeTransform.rotation * angleResult)
                    .Init(gameObject, attackData, ProjectileSpawnPosition, stats.GetModifiedMainStat(MainStat.Damage));

                if(attackData.projectileSpawnType == ProjectileSpawnType.Sequence)
                {
                    float t = attackData.duration / (attackData.projectileNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(attackData.projectileToSpawn, ProjectileSpawnPosition, SpawnRelativeTransform.rotation * Quaternion.AngleAxis(attackData.projectileAngleOffset,transform.forward))
                .Init(gameObject, attackData, ProjectileSpawnPosition, stats.GetModifiedMainStat(MainStat.Damage));
    }

    public void ChangeProjectile(ProjectileController newProjectile)
    {
        attackData.projectileToSpawn = newProjectile;
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
        if (attackData.showDebug)
        {
            Gizmos.color = attackData.meleeDebugColor;
            if (attackData.useMeleeHitbox)
            {

                switch (attackData.hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(HitboxStartPosition, attackData.circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(attackData.debugCube, -1, HitboxStartPosition, HitboxRelativeTransform.rotation, attackData.boxSize);
                        break;

                }

                if (attackData.behaviorType != HitboxBehaviorType.Fixed)
                {
                    switch (attackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(HitboxTargetPosition, attackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(attackData.debugCube, -1, HitboxTargetPosition, HitboxRelativeTransform.rotation, attackData.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector3 hitboxCurrentPos = HitboxStartPosition;
                    float t = Mathf.Clamp01((Time.time - startTime) / stats.GetModifiedSecondaryStat(SecondaryStat.HitboxDuration));
                    switch (attackData.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, attackData.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = HitboxStartPosition - HitboxRelativeTransform.position;
                            Vector3 endVector = HitboxTargetPosition - HitboxRelativeTransform.position;
                            float angleValue = Vector2.Angle(startVector, endVector);
                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, attackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                            hitboxCurrentPos = HitboxRelativeTransform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, attackData.hitboxMovementCurve.Evaluate(t));
                            break;
                    }

                    switch (attackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, attackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(attackData.debugCube, -1, hitboxCurrentPos, HitboxRelativeTransform.rotation, attackData.boxSize);
                            break;
                    }
                }
            }
            Gizmos.color = Color.white;

            Gizmos.color = attackData.projectileDebugColor;
            if (attackData.useProjectile)
            {
                if (attackData.projectileNumber > 1)
                {
                    float minAngle = attackData.spreadAngle / 2f;
                    float angleIncrValue = attackData.spreadAngle / (attackData.projectileNumber - 1);

                    for (int i = 0; i < attackData.projectileNumber; i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleRotation = Quaternion.AngleAxis(angle + attackData.projectileAngleOffset, transform.forward);

                        Vector3 multProjPos = ProjectileSpawnPosition + SpawnRelativeTransform.rotation * angleRotation * Vector3.up * attackData.projectileRange;
                        Vector3 multProjHitboxEndPos = multProjPos + SpawnRelativeTransform.rotation * angleRotation * attackData.projectileToSpawn.HitboxOffset;

                        switch (attackData.projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawSphere(multProjHitboxEndPos, attackData.projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawMesh(attackData.debugCube, -1, multProjHitboxEndPos, SpawnRelativeTransform.rotation * angleRotation, attackData.projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(ProjectileSpawnPosition, multProjPos);
                    }
                }

                else
                {
                    Vector3 singleProjPos = ProjectileSpawnPosition + SpawnRelativeTransform.TransformVector(Quaternion.AngleAxis(attackData.projectileAngleOffset, transform.forward) * Vector3.up * attackData.projectileRange);
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + SpawnRelativeTransform.TransformVector(attackData.projectileToSpawn.HitboxOffset);

                    switch (attackData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(singleProjHitboxCurrentPos, attackData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(attackData.debugCube, -1, singleProjHitboxCurrentPos, SpawnRelativeTransform.rotation * Quaternion.AngleAxis(attackData.projectileAngleOffset, transform.forward), attackData.projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(ProjectileSpawnPosition, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }

}