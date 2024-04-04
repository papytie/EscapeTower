using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackComponent : MonoBehaviour
{
    public bool AttackAvailable => !isAttacking && !isOnCooldown;
    public bool IsOnAttackLag => isOnAttackLag;
    public EnemyAttackData AttackData => currentAttackData;

    [Header("Debug"), Space]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color meleeDebugColor;
    [SerializeField] Color projectileDebugColor;
    [SerializeField] EnemyAttackConfig attackToDebug;
    [SerializeField] EnemyController enemyController; //Set controller ref to debug view in editor

    IAttackFX currentAttackFX;
    EnemyAttackData currentAttackData;
    Animator animator;
    List<PlayerLifeSystem> playerHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;

    public void InitRef(Animator animatorRef, EnemyController controller)
    {
        animator = animatorRef;
        enemyController = controller;
    }

    public void InitAttack(EnemyAttackData attackData, IAttackFX attackFX)
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

        animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.attack);
        currentAttackFX.StartFX(enemyController.CurrentDirection);

    }

    IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(currentAttackData.delay);

        if (currentAttackData.useMeleeHitbox)
            StartMeleeHitboxCheck();

        if (currentAttackData.projectileData.useProjectile)
        {
            StartCoroutine(FireProjectile());
            StartAttackCooldown();
            isAttacking = false;
        }

    }

    bool MeleeHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector2 hitboxCurrentPos = currentAttackData.hitboxPositionOffset;
        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.CurrentDirection);
        Vector2 hitboxStartPos = transform.position + currentRotation * currentAttackData.hitboxPositionOffset;
        Vector2 hitboxEndPos = transform.position + currentRotation * currentAttackData.targetPosition;

        if (currentAttackData.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / currentAttackData.hitboxDuration);

            switch (currentAttackData.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrentPos = Vector2.Lerp(hitboxStartPos, hitboxEndPos, currentAttackData.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = hitboxStartPos - transform.position.ToVector2();
                    Vector3 endVector = hitboxEndPos - transform.position.ToVector2();
                    float angleValue = Vector2.Angle(startVector, endVector);

                    if (currentAttackData.targetPosition.x > currentAttackData.hitboxPositionOffset.x)
                        angleValue *= -1;

                    Vector3 currentVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, currentAttackData.hitboxMovementCurve.Evaluate(t)), base.transform.forward) * startVector);
                    hitboxCurrentPos = transform.position + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, currentAttackData.hitboxMovementCurve.Evaluate(t));
                    break;
            }

        }

        collisionsList = currentAttackData.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrentPos, currentAttackData.circleRadius, Vector2.zero, 0, currentAttackData.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrentPos, currentAttackData.boxSize, Quaternion.Angle(Quaternion.identity, currentRotation), Vector2.zero, 0, currentAttackData.targetLayer),
            _ => null,
        };
        return collisionsList.Length > 0;

    }

    void MeleeHitProcess()
    {
        if (MeleeHitboxCast(out RaycastHit2D[] collisionsList))
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
        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.CurrentDirection);
        Vector3 projectileSpawnPos = transform.position + currentRotation * currentAttackData.projectileData.projectileSpawnOffset;

        if (currentAttackData.projectileData.projectileNumber > 1)
        {
            float minAngle = currentAttackData.projectileData.spreadAngle / 2f;
            float angleIncrValue = currentAttackData.projectileData.spreadAngle / (currentAttackData.projectileData.projectileNumber - 1);

            for (int i = 0; i < currentAttackData.projectileData.projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + currentAttackData.projectileData.projectileAngleOffset, base.transform.forward);

                Instantiate(currentAttackData.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * angleResult)
                    .Init(enemyController.gameObject, currentAttackData.projectileData, projectileSpawnPos, currentAttackData.damage);

                if (currentAttackData.projectileData.projectileSpawnType == ProjectileSpawnType.Sequence)
                {
                    float t = currentAttackData.hitboxDuration / (currentAttackData.projectileData.projectileNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(currentAttackData.projectileData.projectileToSpawn, projectileSpawnPos, currentRotation * Quaternion.AngleAxis(currentAttackData.projectileData.projectileAngleOffset, base.transform.forward))
                .Init(enemyController.gameObject, currentAttackData.projectileData, projectileSpawnPos, currentAttackData.damage);
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
        if (showDebug && attackToDebug.attackData != null)
        {
            Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.CurrentDirection);
            Vector2 hitboxStartPos = transform.position + currentRotation * attackToDebug.attackData.hitboxPositionOffset;
            Vector2 hitboxEndPos = transform.position + currentRotation * attackToDebug.attackData.targetPosition;
            Vector3 projectileSpawnPos = transform.position + currentRotation * attackToDebug.attackData.projectileData.projectileSpawnOffset;

            Gizmos.color = meleeDebugColor;
            if (attackToDebug.attackData.useMeleeHitbox)
            {
                switch (attackToDebug.attackData.hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(hitboxStartPos, attackToDebug.attackData.circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(debugCube, -1, hitboxStartPos, transform.rotation, attackToDebug.attackData.boxSize);
                        break;

                }

                if (attackToDebug.attackData.behaviorType != HitboxBehaviorType.Fixed)
                {
                    switch (attackToDebug.attackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(hitboxEndPos, attackToDebug.attackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, hitboxEndPos, currentRotation, attackToDebug.attackData.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector2 hitboxCurrentPos = attackToDebug.attackData.hitboxPositionOffset;

                    float t = Mathf.Clamp01((Time.time - startTime) / attackToDebug.attackData.hitboxDuration);
                    switch (attackToDebug.attackData.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(hitboxStartPos, hitboxEndPos, attackToDebug.attackData.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = hitboxStartPos - transform.position.ToVector2();
                            Vector3 endVector = hitboxEndPos - transform.position.ToVector2();
                            float angleValue = Vector2.Angle(startVector, endVector);

                            if (attackToDebug.attackData.targetPosition.x > attackToDebug.attackData.hitboxPositionOffset.x)
                                angleValue *= -1;

                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, attackToDebug.attackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                            hitboxCurrentPos = transform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, attackToDebug.attackData.hitboxMovementCurve.Evaluate(t));
                            break;

                    }

                    switch (attackToDebug.attackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, attackToDebug.attackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, hitboxCurrentPos, transform.rotation, attackToDebug.attackData.boxSize);
                            break;

                    }

                }

            }
            Gizmos.color = Color.white;

            Gizmos.color = projectileDebugColor;
            if (attackToDebug.attackData.projectileData.useProjectile)
            {
                if (attackToDebug.attackData.projectileData.projectileNumber > 1)
                {
                    float minAngle = attackToDebug.attackData.projectileData.spreadAngle / 2f;
                    float angleIncrValue = attackToDebug.attackData.projectileData.spreadAngle / (attackToDebug.attackData.projectileData.projectileNumber - 1);

                    for (int i = 0; i < attackToDebug.attackData.projectileData.projectileNumber; i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleRotation = Quaternion.AngleAxis(angle + attackToDebug.attackData.projectileData.projectileAngleOffset, transform.forward);

                        Vector3 multProjPos = projectileSpawnPos + currentRotation * angleRotation * Vector3.up * attackToDebug.attackData.projectileData.projectileRange;
                        Vector3 multProjHitboxEndPos = multProjPos + currentRotation * angleRotation * attackToDebug.attackData.projectileData.projectileToSpawn.HitboxOffset;

                        switch (attackToDebug.attackData.projectileData.projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawWireSphere(multProjHitboxEndPos, attackToDebug.attackData.projectileData.projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawWireMesh(debugCube, -1, multProjHitboxEndPos, currentRotation * angleRotation, attackToDebug.attackData.projectileData.projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(projectileSpawnPos, multProjPos);
                    }

                }

                else
                {
                    Vector3 singleProjPos = projectileSpawnPos + transform.TransformVector(Quaternion.AngleAxis(attackToDebug.attackData.projectileData.projectileAngleOffset, base.transform.forward) * Vector3.up * attackToDebug.attackData.projectileData.projectileRange);
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + transform.TransformVector(attackToDebug.attackData.projectileData.projectileToSpawn.HitboxOffset);

                    switch (attackToDebug.attackData.projectileData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(singleProjHitboxCurrentPos, attackToDebug.attackData.projectileData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, singleProjHitboxCurrentPos, transform.rotation * Quaternion.AngleAxis(attackToDebug.attackData.projectileData.projectileAngleOffset, base.transform.forward), attackToDebug.attackData.projectileData.projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(projectileSpawnPos, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }
}