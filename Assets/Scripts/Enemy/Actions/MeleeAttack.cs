using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time >= cooldownEndTime && Vector3.Distance(transform.position, controller.CurrentTargetPos) <= data.activationRange;
    public bool IsCompleted {get;set;}
    public Vector3 Direction => direction;
    Vector2 direction;

    List<PlayerLifeSystem> playerHit = new();
    EnemyController controller;
    CircleCollider2D circleCollider;
    MeleeData data;
    MeleeAttackEffect effect;

    float detectionEndTime = 0;
    float detectionStartTime = 0;
    float cooldownEndTime = 0;
    Quaternion currentRotation = Quaternion.identity;

    public void Init(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as MeleeData;
        controller = controllerRef;

        if(data.attackEffect != null) 
        { 
            effect = Instantiate(data.attackEffect,controller.transform.position, Quaternion.identity, controller.transform);
            effect.Init(controller, data);
        }
    }

    public void StartProcess()
    {
        StartCoroutine(StartAttackAnim());  
        detectionStartTime = Time.time + data.reactionTime + data.hitbox.delay;
        detectionEndTime = detectionStartTime + data.hitbox.duration;
        direction = (controller.CurrentTargetPos - (circleCollider.transform.position.ToVector2() + circleCollider.offset)).normalized;
        controller.AnimationParam.UpdateMoveAnimDirection(direction * .1f);
        currentRotation = Quaternion.LookRotation(Vector3.forward, direction);
        
    }

    public void UpdateProcess()
    {
        if (Time.time >= detectionEndTime + data.recupTime)
            IsCompleted = true;

        if (Time.time >= detectionStartTime && Time.time <= detectionEndTime)
            MeleeHitDetection();     
    }

    public void EndProcess()
    {
        cooldownEndTime = Time.time + data.cooldown;
        IsCompleted = false;
        playerHit.Clear();
    }

    IEnumerator StartAttackAnim()
    {
        yield return new WaitForSeconds(data.reactionTime);
        if (effect != null)
            effect.Play();
    }

    bool MeleeHitboxCast(out RaycastHit2D[] collisionsList)
    {
        Vector3 center = transform.position.ToVector2() + circleCollider.offset;
        Vector2 hitboxStartPos = center + currentRotation * data.hitbox.startPosOffset;
        Vector2 hitboxEndPos = center + currentRotation * data.hitbox.endPos;

        Vector2 hitboxCurrentPos = center;

        if (data.hitbox.behaviorType != HitboxBehaviorType.Fixed)
        {
            float t = Mathf.Clamp01((Time.time - detectionStartTime) / data.hitbox.duration);

            switch (data.hitbox.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrentPos = Vector2.Lerp(hitboxStartPos, hitboxEndPos, data.hitbox.moveCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector2 startVector = hitboxStartPos - center.ToVector2();
                    Vector2 endVector = hitboxEndPos - center.ToVector2();
                    float angleValue = Vector2.Angle(startVector, endVector);

                    if (data.hitbox.endPos.x > data.hitbox.startPosOffset.x)
                        angleValue *= -1;

                    Vector3 currentVector = Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, data.hitbox.moveCurve.Evaluate(t)), transform.forward) * startVector;
                    hitboxCurrentPos = center + currentVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, data.hitbox.moveCurve.Evaluate(t));
                    break;
            }
        }

        collisionsList = data.hitbox.shape switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrentPos, data.hitbox.circleRadius, Vector2.zero, 0, data.hitbox.detectionLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrentPos, data.hitbox.boxSize, Quaternion.Angle(Quaternion.identity, currentRotation), Vector2.zero, 0, data.hitbox.detectionLayer),
            _ => null,
        };
        return collisionsList.Length > 0;

    }

    void MeleeHitDetection()
    {
        if (MeleeHitboxCast(out RaycastHit2D[] collisionsList))
        {
            foreach (RaycastHit2D collision in collisionsList)
            {
                PlayerLifeSystem playerLifeSystem = collision.transform.GetComponent<PlayerLifeSystem>();

                if (playerLifeSystem && !playerHit.Contains(playerLifeSystem) && playerHit.Count < data.hitbox.maxTargets)
                {
                    playerLifeSystem.TakeDamage(data.baseDamage * controller.Stats.ScalingFactor, collision.normal);
                    playerHit.Add(playerLifeSystem);
                }
            }
        }
    }

    private void OnValidate()
    {
        controller = gameObject.GetComponent<EnemyController>();
        circleCollider = gameObject.GetComponent<CircleCollider2D>();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || controller.MainBehaviour.FSM.CurrentState.Action.GetType() != typeof(MeleeAttack)) return;

        if (data.showDebug && Time.time >= detectionStartTime && Time.time <= detectionEndTime)
        {
            Gizmos.color = data.debugColor;

            Vector3 center = transform.position.ToVector2() + circleCollider.offset;
            Vector2 hitboxCurrentPos = center;
            Vector2 hitboxStartPos = center + currentRotation * data.hitbox.startPosOffset;
            Vector2 hitboxEndPos = transform.position + currentRotation * data.hitbox.endPos;

            float t = Mathf.Clamp01((Time.time - detectionStartTime) / data.hitbox.duration);
            switch (data.hitbox.behaviorType)
            {
                case HitboxBehaviorType.MovingStraight:
                    hitboxCurrentPos = Vector2.Lerp(hitboxStartPos, hitboxEndPos, data.hitbox.moveCurve.Evaluate(t));
                    break;

                case HitboxBehaviorType.MovingOrbital:
                    Vector3 startVector = hitboxStartPos - center.ToVector2();
                    Vector3 endVector = hitboxEndPos - center.ToVector2();
                    float angleValue = Vector2.Angle(startVector, endVector);

                    if (data.hitbox.endPos.x > data.hitbox.startPosOffset.x)
                        angleValue *= -1;

                    Vector3 resultVector = (Quaternion.AngleAxis(Mathf.LerpAngle(0f, angleValue, data.hitbox.moveCurve.Evaluate(t)), gameObject.transform.forward) * startVector);
                    hitboxCurrentPos = center + resultVector.normalized * Mathf.Lerp(startVector.magnitude, endVector.magnitude, data.hitbox.moveCurve.Evaluate(t));
                    break;
            }

            switch (data.hitbox.shape)
            {
                case HitboxShapeType.Circle:
                    Gizmos.DrawSphere(hitboxCurrentPos, data.hitbox.circleRadius);
                    break;

                case HitboxShapeType.Box:
                    Gizmos.DrawMesh(data.debugCube, -1, hitboxCurrentPos, gameObject.transform.rotation, data.hitbox.boxSize);
                    break;
            }

            Gizmos.color = Color.white;
        }
    }
}
