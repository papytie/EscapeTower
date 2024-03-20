using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public EnemyAttackData EnemyAttackData => currentEnemyAttack;
    public Vector3 HitboxStartPosition => transform.position + transform.TransformVector(AttackData.hitboxPositionOffset);
    public Vector3 HitboxTargetPosition => transform.position + transform.TransformVector(AttackData.targetPosition);
    public Vector3 ProjectileSpawnPosition => transform.position + transform.TransformVector(AttackData.projectileSpawnOffset);

    [Header("Debug"), Space]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color meleeDebugColor;
    [SerializeField] Color projectileDebugColor;

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;

    AttackData AttackData => currentEnemyAttack.attackData;
    EnemyAttackData currentEnemyAttack;
    
    List<PlayerLifeSystem> playerHit = new();
    Animator animator;
    EnemyEffectSystem effects;

    public void InitRef(EnemyEffectSystem effectSystem, Animator animatorRef)
    {
        effects = effectSystem;
        animator = animatorRef;

    }

    public void InitData(EnemyAttackData data)
    {
        currentEnemyAttack = data;
        Instantiate(data.attackFX as AttackFXController, transform);
    }

    private void Update()
    {
        if (isOnAttackLag && Time.time >= attackLagEndTime)
            isOnAttackLag = false;

        if (meleeHitboxActive)
        {
            MeleeHitProcess();
            if (Time.time >= startTime + AttackData.hitboxDuration)
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
        effects.AttackFX();

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

    bool WeaponHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector2 hitboxCurrrentPos = HitboxStartPosition;

        if (AttackData.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / AttackData.hitboxDuration);

            switch (AttackData.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, AttackData.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = HitboxStartPosition - transform.position;
                    Vector3 endVector = HitboxTargetPosition - transform.position;
                    float angleValue = Vector2.Angle(startVector, endVector);
                    Vector3 currentVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, AttackData.hitboxMovementCurve.Evaluate(t)), base.transform.forward) * startVector);
                    hitboxCurrrentPos = transform.position + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, AttackData.hitboxMovementCurve.Evaluate(t));
                    break;

            }

        }

        collisionsList = AttackData.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrrentPos, AttackData.circleRadius, Vector2.zero, 0, AttackData.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrrentPos, AttackData.boxSize, Quaternion.Angle(Quaternion.identity, transform.transform.rotation), Vector2.zero, 0, AttackData.targetLayer),
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

                if (playerLifeSystem && !playerHit.Contains(playerLifeSystem) && playerHit.Count < AttackData.maxTargets)
                {
                    playerLifeSystem.TakeDamage(AttackData.damage);
                    playerHit.Add(playerLifeSystem);
                }

            }

        }

    }

    IEnumerator FireProjectile()
    {

        if (AttackData.projectileNumber > 1)
        {
            float minAngle = AttackData.spreadAngle / 2f;
            float angleIncrValue = AttackData.spreadAngle / (AttackData.projectileNumber - 1);

            for (int i = 0; i < AttackData.projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + AttackData.projectileAngleOffset, base.transform.forward);

                Instantiate(AttackData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * angleResult)
                    .Init(gameObject, AttackData, ProjectileSpawnPosition, AttackData.damage);

                if (AttackData.projectileSpawnType == ProjectileSpawnType.Sequence)
                {
                    float t = AttackData.hitboxDuration / (AttackData.projectileNumber - 1);
                    yield return new WaitForSeconds(t);
                }

            }

        }
        else Instantiate(AttackData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * Quaternion.AngleAxis(AttackData.projectileAngleOffset, base.transform.forward))
                .Init(gameObject, AttackData, ProjectileSpawnPosition, AttackData.damage);
    }

    public void ChangeProjectile(ProjectileController newProjectile)
    {
        AttackData.projectileToSpawn = newProjectile;

    }

    void StartAttackCooldown()
    {
        cooldownEndTime = Time.time + AttackData.cooldown;
        isOnCooldown = true;

    }

    void StartMeleeHitboxCheck()
    {
        startTime = Time.time;
        meleeHitboxActive = true;

    }

    void StartAttackLag()
    {
        attackLagEndTime = Time.time + AttackData.lag;
        isOnAttackLag = true;

    }

    private void OnDrawGizmos()
    {
        if (showDebug && Application.isPlaying && EnemyAttackData != null)
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
                        Gizmos.DrawWireMesh(debugCube, -1, HitboxStartPosition, transform.rotation, AttackData.boxSize);
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
                            Gizmos.DrawWireMesh(debugCube, -1, HitboxTargetPosition, transform.rotation, AttackData.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector3 hitboxCurrentPos = HitboxStartPosition;
                    float t = Mathf.Clamp01((Time.time - startTime) / AttackData.hitboxDuration);
                    switch (AttackData.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, AttackData.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = HitboxStartPosition - transform.position;
                            Vector3 endVector = HitboxTargetPosition - transform.position;
                            float angleValue = Vector2.Angle(startVector, endVector);
                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, AttackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                            hitboxCurrentPos = transform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, AttackData.hitboxMovementCurve.Evaluate(t));
                            break;

                    }

                    switch (AttackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, AttackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, hitboxCurrentPos, transform.rotation, AttackData.boxSize);
                            break;

                    }

                }

            }
            Gizmos.color = Color.white;

            Gizmos.color = projectileDebugColor;
            if (AttackData.useProjectile)
            {
                if (AttackData.projectileNumber > 1)
                {
                    float minAngle = AttackData.spreadAngle / 2f;
                    float angleIncrValue = AttackData.spreadAngle / (AttackData.projectileNumber - 1);

                    for (int i = 0; i < AttackData.projectileNumber; i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleRotation = Quaternion.AngleAxis(angle + AttackData.projectileAngleOffset, transform.forward);

                        Vector3 multProjPos = ProjectileSpawnPosition + transform.rotation * angleRotation * Vector3.up * AttackData.projectileRange;
                        Vector3 multProjHitboxEndPos = multProjPos + transform.rotation * angleRotation * AttackData.projectileToSpawn.HitboxOffset;

                        switch (AttackData.projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawWireSphere(multProjHitboxEndPos, AttackData.projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawWireMesh(debugCube, -1, multProjHitboxEndPos, transform.rotation * angleRotation, AttackData.projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(ProjectileSpawnPosition, multProjPos);
                    }

                }

                else
                {
                    Vector3 singleProjPos = ProjectileSpawnPosition + transform.TransformVector(Quaternion.AngleAxis(AttackData.projectileAngleOffset, base.transform.forward) * Vector3.up * AttackData.projectileRange);
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + transform.TransformVector(AttackData.projectileToSpawn.HitboxOffset);

                    switch (AttackData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(singleProjHitboxCurrentPos, AttackData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(debugCube, -1, singleProjHitboxCurrentPos, transform.rotation * Quaternion.AngleAxis(AttackData.projectileAngleOffset, base.transform.forward), AttackData.projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(ProjectileSpawnPosition, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }
}
