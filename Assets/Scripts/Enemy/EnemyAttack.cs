using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public AttackData AttackData => attackData;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color meleeDebugColor;
    [SerializeField] Color projectileDebugColor;

    List<PlayerLifeSystem> playerHit = new();

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;

    public Vector3 HitboxStartPosition => transform.position + transform.TransformVector(attackData.hitboxPositionOffset);
    public Vector3 HitboxTargetPosition => transform.position + transform.TransformVector(attackData.targetPosition);
    public Vector3 ProjectileSpawnPosition => transform.position + transform.TransformVector(attackData.projectileSpawnOffset);

    Animator animator;
    EnemyEffectSystem effects;
    [SerializeField] AttackData attackData;

    public void InitRef(EnemyEffectSystem effectSystem, Animator animatorRef)
    {
        effects = effectSystem;
        animator = animatorRef;
    }

    public void InitData(AttackData data)
    {
        attackData = data;
    }

    private void Update()
    {
        if (isOnAttackLag && Time.time >= attackLagEndTime)
            isOnAttackLag = false;

        if (meleeHitboxActive)
        {
            MeleeHitProcess();
            if (Time.time >= startTime + attackData.hitboxDuration)
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

    bool WeaponHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector2 hitboxCurrrentPos = HitboxStartPosition;

        if (attackData.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / attackData.hitboxDuration);

            switch (attackData.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, attackData.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = HitboxStartPosition - transform.position;
                    Vector3 endVector = HitboxTargetPosition - transform.position;
                    float angleValue = Vector2.Angle(startVector, endVector);
                    Vector3 currentVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, attackData.hitboxMovementCurve.Evaluate(t)), base.transform.forward) * startVector);
                    hitboxCurrrentPos = transform.position + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, attackData.hitboxMovementCurve.Evaluate(t));
                    break;
            }

        }

        collisionsList = attackData.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrrentPos, attackData.circleRadius, Vector2.zero, 0, attackData.targetLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrrentPos, attackData.boxSize, Quaternion.Angle(Quaternion.identity, transform.transform.rotation), Vector2.zero, 0, attackData.targetLayer),
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

                if (playerLifeSystem && !playerHit.Contains(playerLifeSystem) && playerHit.Count < attackData.maxTargets)
                {
                    playerLifeSystem.TakeDamage(attackData.damage);
                    playerHit.Add(playerLifeSystem);
                }
            }
        }
    }

    IEnumerator FireProjectile()
    {

        if (attackData.projectileNumber > 1)
        {
            float minAngle = attackData.spreadAngle / 2f;
            float angleIncrValue = attackData.spreadAngle / (attackData.projectileNumber - 1);

            for (int i = 0; i < attackData.projectileNumber; i++)
            {
                float angle = minAngle - i * angleIncrValue;
                Quaternion angleResult = Quaternion.AngleAxis(angle + attackData.projectileAngleOffset, base.transform.forward);

                Instantiate(attackData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * angleResult)
                    .Init(gameObject, attackData, ProjectileSpawnPosition, attackData.damage /*TODO: Get Modified stats*/);

                if (attackData.projectileSpawnType == ProjectileSpawnType.Sequence)
                {
                    float t = attackData.hitboxDuration / (attackData.projectileNumber - 1);
                    yield return new WaitForSeconds(t);
                }
            }
        }
        else Instantiate(attackData.projectileToSpawn, ProjectileSpawnPosition, transform.rotation * Quaternion.AngleAxis(attackData.projectileAngleOffset, base.transform.forward))
                .Init(gameObject, attackData, ProjectileSpawnPosition, attackData.damage /*TODO: Get Modified stats*/);
    }

    public void ChangeProjectile(ProjectileController newProjectile)
    {
        attackData.projectileToSpawn = newProjectile;
    }

    void StartAttackCooldown()
    {
        cooldownEndTime = Time.time + attackData.cooldown;
        isOnCooldown = true;
    }

    void StartMeleeHitboxCheck()
    {
        startTime = Time.time;
        meleeHitboxActive = true;
    }

    void StartAttackLag()
    {
        attackLagEndTime = Time.time + attackData.lag;
        isOnAttackLag = true;
    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = meleeDebugColor;
            if (attackData.useMeleeHitbox)
            {

                switch (attackData.hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(HitboxStartPosition, attackData.circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(debugCube, -1, HitboxStartPosition, transform.rotation, attackData.boxSize);
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
                            Gizmos.DrawWireMesh(debugCube, -1, HitboxTargetPosition, transform.rotation, attackData.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector3 hitboxCurrentPos = HitboxStartPosition;
                    float t = Mathf.Clamp01((Time.time - startTime) / attackData.hitboxDuration);
                    switch (attackData.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(HitboxStartPosition, HitboxTargetPosition, attackData.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = HitboxStartPosition - transform.position;
                            Vector3 endVector = HitboxTargetPosition - transform.position;
                            float angleValue = Vector2.Angle(startVector, endVector);
                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, attackData.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                            hitboxCurrentPos = transform.position + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, attackData.hitboxMovementCurve.Evaluate(t));
                            break;
                    }

                    switch (attackData.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, attackData.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, hitboxCurrentPos, transform.rotation, attackData.boxSize);
                            break;
                    }
                }
            }
            Gizmos.color = Color.white;

            Gizmos.color = projectileDebugColor;
            if (attackData.useProjectile)
            {
                if (attackData.projectileNumber > 1)
                {
                    float minAngle = attackData.spreadAngle / 2f;
                    float angleIncrValue = attackData.spreadAngle / (attackData.projectileNumber - 1);

                    for (int i = 0; i < attackData.projectileNumber; i++)
                    {
                        float angle = minAngle - i * angleIncrValue;
                        Quaternion angleResult = Quaternion.AngleAxis(angle + attackData.projectileAngleOffset, base.transform.forward);

                        Vector3 multProjPos = ProjectileSpawnPosition + transform.rotation * angleResult * Vector3.up * attackData.projectileRange;
                        Vector3 multProjHitboxCurrentPos = multProjPos + transform.TransformVector(angleResult * attackData.projectileToSpawn.HitboxOffset);

                        switch (attackData.projectileToSpawn.HitboxShape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawSphere(multProjHitboxCurrentPos, attackData.projectileToSpawn.CircleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawMesh(debugCube, -1, multProjHitboxCurrentPos, transform.rotation * angleResult, attackData.projectileToSpawn.BoxSize);
                                break;

                        }
                        Gizmos.DrawLine(ProjectileSpawnPosition, multProjPos);
                    }
                }

                else
                {
                    Vector3 singleProjPos = ProjectileSpawnPosition + transform.TransformVector(Quaternion.AngleAxis(attackData.projectileAngleOffset, base.transform.forward) * Vector3.up * attackData.projectileRange);
                    Vector3 singleProjHitboxCurrentPos = singleProjPos + transform.TransformVector(attackData.projectileToSpawn.HitboxOffset);

                    switch (attackData.projectileToSpawn.HitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(singleProjHitboxCurrentPos, attackData.projectileToSpawn.CircleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(debugCube, -1, singleProjHitboxCurrentPos, transform.rotation * Quaternion.AngleAxis(attackData.projectileAngleOffset, base.transform.forward), attackData.projectileToSpawn.BoxSize);
                            break;

                    }
                    Gizmos.DrawLine(ProjectileSpawnPosition, singleProjPos);

                }

            }
            Gizmos.color = Color.white;
        }

    }
}
