using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackComponent : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsOnAttackLag => isOnAttackLag;
    public AttackData AttackData => currentAttackData;
    public Vector3 HitboxStartPosition => transform.position + transform.TransformVector(currentAttackData.hitboxPositionOffset);
    public Vector3 HitboxTargetPosition => transform.position + transform.TransformVector(currentAttackData.targetPosition);
    public Vector3 ProjectileSpawnPosition => transform.position + transform.TransformVector(currentAttackData.projectileSpawnOffset);

    [Header("Debug"), Space]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color meleeDebugColor;
    [SerializeField] Color projectileDebugColor;

    IAttackFX currentAttackFX;
    AttackData currentAttackData;
    Animator animator;
    List<PlayerLifeSystem> playerHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;

    public void InitRef(Animator animatorRef)
    {
        animator = animatorRef;

    }

    public void InitAttack(AttackData attackData, IAttackFX attackFX)
    {
        if (currentAttackData != attackData)
            currentAttackData = attackData;
        if (currentAttackData == null)
            Debug.LogWarning("currentAttackData is null or invalid!");

        if (currentAttackFX != attackFX)
            currentAttackFX = attackFX;
        if (currentAttackFX == null)
            Debug.LogWarning("currentAttackFX is null or invalid!");
    }

    private void Update()
    {
        if (isOnAttackLag && Time.time >= attackLagEndTime)
            isOnAttackLag = false;

        if (meleeHitboxActive)
        {
            MeleeHitProcess();
            if (Time.time >= startTime + currentAttackData.hitboxDuration)
            {
                meleeHitboxActive = false;
                isAttacking = false;
                playerHit.Clear();
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
        StartCoroutine(AttackProcess());

        animator.SetTrigger(GameParams.Animation.ENEMY_ATTACK_TRIGGER);
        currentAttackFX.StartFX();

    }

    IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(currentAttackData.delay);

        if (currentAttackData.useMeleeHitbox)
            StartMeleeHitboxCheck();

        if (currentAttackData.useProjectile)
        {
            StartCoroutine(FireProjectile());
            StartAttackCooldown();
            isAttacking = false;
        }

    }

    bool WeaponHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector2 hitboxCurrrentPos = HitboxStartPosition;

        if (currentAttackData.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / currentAttackData.hitboxDuration);

            switch (currentAttackData.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, currentAttackData.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = HitboxStartPosition - transform.position;
                    Vector3 endVector = HitboxTargetPosition - transform.position;
                    float angleValue = Vector2.Angle(startVector, endVector);
                    Vector3 currentVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, currentAttackData.hitboxMovementCurve.Evaluate(t)), base.transform.forward) * startVector);
                    hitboxCurrrentPos = transform.position + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, currentAttackData.hitboxMovementCurve.Evaluate(t));
                    break;


            }

        }

        collisionsList = currentAttackData.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrrentPos, currentAttackData.circleRadius, Vector2.zero, 0, currentAttackData.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrrentPos, currentAttackData.boxSize, Quaternion.Angle(Quaternion.identity, transform.transform.rotation), Vector2.zero, 0, currentAttackData.targetLayer),
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
                PlayerLifeSystem playerLifeSystem = collision.transform.GetComponent<PlayerLifeSystem>();

                if (playerLifeSystem && !playerHit.Contains(playerLifeSystem) && playerHit.Count < currentAttackData.maxTargets)
                {
                    playerLifeSystem.TakeDamage(currentAttackData.damage);
                    playerHit.Add(playerLifeSystem);
                }

            }

        }

    }

    IEnumerator FireProjectile()
    {

        if (currentAttackData.projectileNumber > 1)
        {
            float minAngle = currentAttackData.spreadAngle / 2f;
            float angleIncrValue = currentAttackData.spreadAngle / (currentAttackData.projectileNumber - 1);

            for (int i = 0; i < currentAttackData.projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + currentAttackData.projectileAngleOffset, base.transform.forward);

                Instantiate(currentAttackData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * angleResult)
                    .Init(gameObject, currentAttackData, ProjectileSpawnPosition, currentAttackData.damage);

                if (currentAttackData.projectileSpawnType == ProjectileSpawnType.Sequence)
                {
                    float t = currentAttackData.hitboxDuration / (currentAttackData.projectileNumber - 1);
                    yield return new WaitForSeconds(t);
                }

            }

        }
        else Instantiate(currentAttackData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * Quaternion.AngleAxis(currentAttackData.projectileAngleOffset, base.transform.forward))
                .Init(gameObject, currentAttackData, ProjectileSpawnPosition, currentAttackData.damage);
    }

    public void ChangeProjectile(ProjectileController newProjectile)
    {
        currentAttackData.projectileToSpawn = newProjectile;

    }

    void StartAttackCooldown()
    {
        cooldownEndTime = Time.time + currentAttackData.cooldown;
        isOnCooldown = true;

    }

    void StartMeleeHitboxCheck()
    {
        startTime = Time.time;
        meleeHitboxActive = true;

    }

    void StartAttackLag()
    {
        attackLagEndTime = Time.time + currentAttackData.lag;
        isOnAttackLag = true;

    }

    private void OnDrawGizmos()
    {
        if (showDebug && Application.isPlaying && currentAttackData != null)
        {
            Gizmos.color = meleeDebugColor;
            if (currentAttackData.useMeleeHitbox)
            {
                switch (currentAttackData.hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(HitboxStartPosition, currentAttackData.circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(debugCube, -1, HitboxStartPosition, transform.rotation, currentAttackData.boxSize);
                        break;

                }

                if (currentAttackData.behaviorType != HitboxBehaviorType.Fixed)
                {
                    switch (currentAttackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(HitboxTargetPosition, currentAttackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, HitboxTargetPosition, transform.rotation, currentAttackData.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector3 hitboxCurrentPos = HitboxStartPosition;
                    float t = Mathf.Clamp01((Time.time - startTime) / currentAttackData.hitboxDuration);
                    switch (currentAttackData.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, currentAttackData.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = HitboxStartPosition - transform.position;
                            Vector3 endVector = HitboxTargetPosition - transform.position;
                            float angleValue = Vector2.Angle(startVector, endVector);
                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, currentAttackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                            hitboxCurrentPos = transform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, currentAttackData.hitboxMovementCurve.Evaluate(t));
                            break;

                    }

                    switch (currentAttackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, currentAttackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, hitboxCurrentPos, transform.rotation, currentAttackData.boxSize);
                            break;

                    }

                }

            }
            Gizmos.color = Color.white;

            Gizmos.color = projectileDebugColor;
            if (currentAttackData.useProjectile)
            {
                if (currentAttackData.projectileNumber > 1)
                {
                    float minAngle = currentAttackData.spreadAngle / 2f;
                    float angleIncrValue = currentAttackData.spreadAngle / (currentAttackData.projectileNumber - 1);

                    for (int i = 0; i < currentAttackData.projectileNumber; i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleRotation = Quaternion.AngleAxis(angle + currentAttackData.projectileAngleOffset, transform.forward);

                        Vector3 multProjPos = ProjectileSpawnPosition + transform.rotation * angleRotation * Vector3.up * currentAttackData.projectileRange;
                        Vector3 multProjHitboxEndPos = multProjPos + transform.rotation * angleRotation * currentAttackData.projectileToSpawn.HitboxOffset;

                        switch (currentAttackData.projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawWireSphere(multProjHitboxEndPos, currentAttackData.projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawWireMesh(debugCube, -1, multProjHitboxEndPos, transform.rotation * angleRotation, currentAttackData.projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(ProjectileSpawnPosition, multProjPos);
                    }

                }

                else
                {
                    Vector3 singleProjPos = ProjectileSpawnPosition + transform.TransformVector(Quaternion.AngleAxis(currentAttackData.projectileAngleOffset, base.transform.forward) * Vector3.up * currentAttackData.projectileRange);
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + transform.TransformVector(currentAttackData.projectileToSpawn.HitboxOffset);

                    switch (currentAttackData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(singleProjHitboxCurrentPos, currentAttackData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, singleProjHitboxCurrentPos, transform.rotation * Quaternion.AngleAxis(currentAttackData.projectileAngleOffset, base.transform.forward), currentAttackData.projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(ProjectileSpawnPosition, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }
}
