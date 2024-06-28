using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= /*endCDTime*/ 0;

    public bool IsCompleted => throw new System.NotImplementedException();

    IAttackFX attackFX;
    EnemyStatsComponent stats;
    Animator animator;
    List<PlayerLifeSystem> playerHit = new();
    EnemyController controller; //Set controller ref to debug view in editor
    CircleCollider2D circleCollider;

    float cooldownEndTime = 0;
    bool isOnCooldown = false;
    float attackLagEndTime = 0;
    bool isOnAttackLag = false;
    float startTime = 0;
    bool meleeHitboxActive = false;
    bool isAttacking = false;
    Quaternion currentRotation = Quaternion.identity;

    private MeleeData data;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as MeleeData;
        controller = controllerRef;
    }

    private void Update()
    {    
        if (isOnAttackLag && Time.time >= attackLagEndTime)
            isOnAttackLag = false;

        if (meleeHitboxActive)
        {
            MeleeHitProcess();
            if (Time.time >= startTime + data.hitboxSettings.hitboxDuration)
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
        //attackFX.StartFX(enemyController.CurrentDirection);

        //For Debug infos
        currentRotation = Quaternion.LookRotation(Vector3.forward, controller.CurrentDirection);
    }

    IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(data.delay);
        StartMeleeHitboxCheck();
    }

    bool MeleeHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector3 center = transform.position.ToVector2() + circleCollider.offset;
        Vector2 hitboxCurrentPos = data.hitboxSettings.hitboxStartPosOffset;
        Vector2 hitboxStartPos = center + currentRotation * data.hitboxSettings.hitboxStartPosOffset;
        Vector2 hitboxEndPos = transform.position + currentRotation * data.hitboxSettings.hitboxEndPos;

        if (data.hitboxSettings.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / data.hitboxSettings.hitboxDuration);

            switch (data.hitboxSettings.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrentPos = Vector2.Lerp(hitboxStartPos, hitboxEndPos, data.hitboxSettings.hitboxMovementCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = hitboxStartPos - center.ToVector2();
                    Vector3 endVector = hitboxEndPos - center.ToVector2();
                    float angleValue = Vector2.Angle(startVector, endVector);

                    if (data.hitboxSettings.hitboxEndPos.x > data.hitboxSettings.hitboxStartPosOffset.x)
                        angleValue *= -1;

                    Vector3 currentVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, data.hitboxSettings.hitboxMovementCurve.Evaluate(t)), transform.forward) * startVector);
                    hitboxCurrentPos = center + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, data.hitboxSettings.hitboxMovementCurve.Evaluate(t));
                    break;
            }
        }

        collisionsList = data.hitboxSettings.hitboxShape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrentPos, data.hitboxSettings.circleRadius, Vector2.zero, 0, data.hitboxSettings.activeLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrentPos, data.hitboxSettings.boxSize, Quaternion.Angle(Quaternion.identity, currentRotation), Vector2.zero, 0, data.hitboxSettings.activeLayer),
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

                if (playerLifeSystem && !playerHit.Contains(playerLifeSystem) && playerHit.Count < data.hitboxSettings.maxTargets)
                {
                    playerLifeSystem.TakeDamage(stats.MeleeDamage, collision.normal);
                    playerHit.Add(playerLifeSystem);
                }
            }
        }
    }


    void StartAttackCooldown()
    {
        cooldownEndTime = Time.time + data.cooldown;
        isOnCooldown = true;

    }

    void StartMeleeHitboxCheck()
    {
        startTime = Time.time;
        meleeHitboxActive = true;
    }

    void StartAttackLag()
    {
        attackLagEndTime = Time.time + data.lag;
        isOnAttackLag = true;
    }

    private void OnValidate()
    {
        controller = gameObject.GetComponent<EnemyController>();
        circleCollider = gameObject.GetComponent<CircleCollider2D>();
    }


    public void StartProcess()
    {
        isAttacking = true;
        StartAttackLag();
        StartCoroutine(AttackProcess());

        animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.attack);
        //attackFX.StartFX(enemyController.CurrentDirection);

        //For Debug infos
        currentRotation = Quaternion.LookRotation(Vector3.forward, controller.CurrentDirection);

    }

    public void UpdateProcess()
    {
        if (isOnCooldown && Time.time >= cooldownEndTime)
            isOnCooldown = false;

        MeleeHitProcess();
        if (Time.time >= startTime + data.hitboxSettings.hitboxDuration)
        {
            meleeHitboxActive = false;
            isAttacking = false;
            playerHit.Clear();
            StartAttackCooldown();
        }

    }

    public void EndProcess()
    {
        throw new System.NotImplementedException();
    }

    /*    private void OnDrawGizmos()
        {
            if (meleeAttackData.hitboxSettings.showDebug && meleeAttackData != null)
            {
                Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.CurrentDirection);
                Vector3 center = transform.position.ToVector2() + circleCollider.offset;
                Vector2 hitboxStartPos = center + currentRotation * meleeAttackData.hitboxSettings.hitboxStartPosOffset;
                Vector2 hitboxEndPos = center + currentRotation * meleeAttackData.hitboxSettings.hitboxEndPos;

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(center, 0.02f);

                Gizmos.color = meleeAttackData.hitboxSettings.meleeDebugColor;
                switch (meleeAttackData.hitboxSettings.hitboxShape)
                {
                    case HitboxShapeType.Circle:
                        Gizmos.DrawWireSphere(hitboxStartPos, meleeAttackData.hitboxSettings.circleRadius);
                        break;

                    case HitboxShapeType.Box:
                        Gizmos.DrawWireMesh(meleeAttackData.hitboxSettings.debugCube, -1, hitboxStartPos, gameObject.transform.rotation, meleeAttackData.hitboxSettings.boxSize);
                        break;

                }

                if (meleeAttackData.hitboxSettings.behaviorType != HitboxBehaviorType.Fixed)
                {
                    switch (meleeAttackData.hitboxSettings.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawWireSphere(hitboxEndPos, meleeAttackData.hitboxSettings.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawWireMesh(meleeAttackData.hitboxSettings.debugCube, -1, hitboxEndPos, currentRotation, meleeAttackData.hitboxSettings.boxSize);
                            break;

                    }

                }

                if (meleeHitboxActive && Application.isPlaying)
                {
                    Vector2 hitboxCurrentPos = meleeAttackData.hitboxSettings.hitboxStartPosOffset;

                    float t = Mathf.Clamp01((Time.time - startTime) / meleeAttackData.hitboxSettings.hitboxDuration);
                    switch (meleeAttackData.hitboxSettings.behaviorType)
                    {
                        case HitboxBehaviorType.MovingStraight:
                            hitboxCurrentPos = Vector2.Lerp(hitboxStartPos, hitboxEndPos, meleeAttackData.hitboxSettings.hitboxMovementCurve.Evaluate(t));
                            break;

                        case HitboxBehaviorType.MovingOrbital:
                            Vector3 startVector = hitboxStartPos - center.ToVector2();
                            Vector3 endVector = hitboxEndPos - center.ToVector2();
                            float angleValue = Vector2.Angle(startVector, endVector);

                            if (meleeAttackData.hitboxSettings.hitboxEndPos.x > meleeAttackData.hitboxSettings.hitboxStartPosOffset.x)
                                angleValue *= -1;

                            Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, meleeAttackData.hitboxSettings.hitboxMovementCurve.Evaluate(t)), gameObject.transform.forward) * startVector);
                            hitboxCurrentPos = center + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, meleeAttackData.hitboxSettings.hitboxMovementCurve.Evaluate(t));
                            break;

                    }

                    switch (meleeAttackData.hitboxSettings.hitboxShape)
                    {
                        case HitboxShapeType.Circle:
                            Gizmos.DrawSphere(hitboxCurrentPos, meleeAttackData.hitboxSettings.circleRadius);
                            break;

                        case HitboxShapeType.Box:
                            Gizmos.DrawMesh(meleeAttackData.hitboxSettings.debugCube, -1, hitboxCurrentPos, gameObject.transform.rotation, meleeAttackData.hitboxSettings.boxSize);
                            break;
                    }
                }
            }
        }
    */
}
