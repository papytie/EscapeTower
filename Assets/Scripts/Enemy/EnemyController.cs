using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionComponent), typeof(EnemyDetectionComponent), typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyStatsComponent), typeof(EnemyLifeSystemComponent), typeof(EnemyDropComponent))] 
[RequireComponent(typeof(EnemyLootComponent), typeof(EnemyAnimationComponent))]

public class EnemyController : MonoBehaviour
{
    public EnemyDropComponent DropComponent => drop;
    public EnemyStatsComponent Stats => stats;
    public EnemyLifeSystemComponent LifeSystem => lifeSystem;
    public EnemyDetectionComponent Detection => detection;
    public EnemyLootComponent LootSystem => lootSystem;
    public EnemyAnimationComponent AnimationParam => animationParam;
    public CollisionComponent Collision => collision;
    public Animator Animator => animator;
    public CircleCollider2D CircleCollider => circleCollider;
    public IBehaviour MainBehaviour => mainBehaviour;
    public IBehaviour AdditiveBehaviours => additiveBehaviour;
    public bool TargetAcquired => currentTarget != null;
    public Vector2 CurrentTargetPos => currentTargetPos;
    public Vector2 CurrentDirection { get; set; }
    public GameObject CurrentTarget => currentTarget;

    [SerializeField] BehaviourConfig mainBehaviourConfig;
    [SerializeField] Mesh debugCube;
    
    IBehaviour mainBehaviour;
    IBehaviour additiveBehaviour;
    Vector2 currentTargetPos;

    GameObject currentTarget;
    EnemyStatsComponent stats;
    EnemyLifeSystemComponent lifeSystem;
    EnemyDetectionComponent detection;
    EnemyLootComponent lootSystem;
    EnemyAnimationComponent animationParam;
    EnemyDropComponent drop;
    CollisionComponent collision;
    Animator animator;
    CircleCollider2D circleCollider;

    private void Awake()
    {
        if (!TryGetComponent<Animator>(out animator)) animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogWarning("Animator is missing!");

        circleCollider = GetComponent<CircleCollider2D>();
        stats = GetComponent<EnemyStatsComponent>();
        lifeSystem = GetComponent<EnemyLifeSystemComponent>();
        detection = GetComponent<EnemyDetectionComponent>();
        lootSystem = GetComponent<EnemyLootComponent>();
        animationParam = GetComponent<EnemyAnimationComponent>();
        drop = GetComponent<EnemyDropComponent>();
        collision = GetComponent<CollisionComponent>();
    }

    private void Start()
    {
        lifeSystem.Init(this);
        stats.Init();
        detection.Init(this);
        animationParam.Init(this);
        drop.Init(this);
        collision.Init();

        if (mainBehaviourConfig != null)
        {
            mainBehaviourConfig.data.InitActionsList();
            mainBehaviour = BehaviourFactory.Create(gameObject, mainBehaviourConfig.behaviourType);
            mainBehaviour.Init(this, mainBehaviourConfig.data);
        }
        else { Debug.LogWarning("Main Behaviour Config is missing!"); return; }

        EnemyManager.Instance.AddEnemy(this);
    }

    private void Update()
    {
        //Detection system used to raycast target
        if (detection.PlayerDetection(out GameObject target))
        {
            //if a valid target is cast then set the variable
            if (currentTarget == null || currentTarget != target)
            {
                currentTarget = target;
            }
            //Update the position of the target
            currentTargetPos = target.transform.position;
        }
        else if (currentTarget != null) currentTarget = null;

        mainBehaviour.UpdateFSM();
    }

    private void OnValidate()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        circleCollider = GetComponent<CircleCollider2D>();
        stats = GetComponent<EnemyStatsComponent>();
        lifeSystem = GetComponent<EnemyLifeSystemComponent>();
        detection = GetComponent<EnemyDetectionComponent>();
        lootSystem = GetComponent<EnemyLootComponent>();
        collision = GetComponent<CollisionComponent>();
        animationParam = GetComponent<EnemyAnimationComponent>();

        lifeSystem.Init(this);
        detection.Init(this);
        collision.Init();
        stats.Init();
        animationParam.Init(this);
    }

    private void OnDrawGizmos()
    {
        foreach (ActionConfig actionConfig in mainBehaviourConfig.data.Actions)
        {
            switch (actionConfig.actionType)
            {
                case ActionType.MeleeAttack:
                    MeleeData meleeData = (MeleeData)actionConfig.data;

                    if (meleeData.showDebug && meleeData != null)
                    {
                        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, new(0f,-1f));
                        if (Application.isPlaying)
                            currentRotation = Quaternion.LookRotation(Vector3.forward, mainBehaviour.FSM.CurrentState.Action.Direction);

                        Vector3 center = transform.position.ToVector2() + circleCollider.offset;
                        Vector2 hitboxStartPos = center + currentRotation * meleeData.hitbox.startPosOffset;
                        Vector2 hitboxEndPos = center + currentRotation * meleeData.hitbox.endPos;

                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(center, 0.02f);

                        Gizmos.color = meleeData.debugColor;
                        Gizmos.DrawWireSphere(center, meleeData.activationRange);

                        switch (meleeData.hitbox.shape)
                        {
                            case HitboxShapeType.Circle:
                                Gizmos.DrawWireSphere(hitboxStartPos, meleeData.hitbox.circleRadius);
                                break;

                            case HitboxShapeType.Box:
                                Gizmos.DrawWireMesh(meleeData.debugCube, -1, hitboxStartPos, gameObject.transform.rotation, meleeData.hitbox.boxSize);
                                break;
                        }

                        if (meleeData.hitbox.behaviorType != HitboxBehaviorType.Fixed)
                        {
                            switch (meleeData.hitbox.shape)
                            {
                                case HitboxShapeType.Circle:
                                    Gizmos.DrawWireSphere(hitboxEndPos, meleeData.hitbox.circleRadius);
                                    break;

                                case HitboxShapeType.Box:
                                    Gizmos.DrawWireMesh(meleeData.debugCube, -1, hitboxEndPos, currentRotation, meleeData.hitbox.boxSize);
                                    break;
                            }
                        }
                    }
                    break;

                case ActionType.RangedAttack:
                    RangedData rangedData = (RangedData)actionConfig.data;

                    if (rangedData.showDebug && rangedData != null && rangedData.projectileData != null)
                    {
                        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, new(0f, -1f));
                        if (Application.isPlaying)
                            currentRotation = Quaternion.LookRotation(Vector3.forward, mainBehaviour.FSM.CurrentState.Action.Direction);

                        Vector3 center = transform.position.ToVector2() + circleCollider.offset;
                        Vector3 projectileSpawnPos = center + currentRotation * rangedData.projectileData.spawnOffset;

                        Gizmos.color = rangedData.debugColor;
                        Gizmos.DrawWireSphere(projectileSpawnPos, .02f);
                        if (rangedData.projectileData.spawnNumber > 1)
                        {
                            float minAngle = rangedData.projectileData.spreadAngle / 2f;
                            float angleIncrValue = rangedData.projectileData.spreadAngle / (rangedData.projectileData.spawnNumber - 1);

                            for (int i = 0; i < rangedData.projectileData.spawnNumber; i++)
                            {
                                float angle = minAngle - i * angleIncrValue;
                                Quaternion angleRotation = Quaternion.AngleAxis(angle + rangedData.projectileData.angleOffset, gameObject.transform.forward);

                                Vector3 multProjPos = projectileSpawnPos + currentRotation * angleRotation * Vector3.up * rangedData.projectileData.range;
                                Vector3 multProjHitboxEndPos = multProjPos + currentRotation * angleRotation * rangedData.projectileData.projectileToSpawn.HitboxOffset;

                                switch (rangedData.projectileData.projectileToSpawn.HitboxShape)
                                {
                                    case HitboxShapeType.Circle:
                                        Gizmos.DrawWireSphere(multProjHitboxEndPos, rangedData.projectileData.projectileToSpawn.CircleRadius);
                                        break;

                                    case HitboxShapeType.Box:
                                        Gizmos.DrawWireMesh(debugCube, -1, multProjHitboxEndPos, currentRotation * angleRotation, rangedData.projectileData.projectileToSpawn.BoxSize);
                                        break;
                                }
                                Gizmos.DrawLine(projectileSpawnPos, multProjPos);
                            }
                        }
                        else
                        {
                            Vector3 singleProjEndPos = projectileSpawnPos + currentRotation * Vector3.up * rangedData.projectileData.range;
                            if (rangedData.projectileData.projectileToSpawn == null) continue;

                            switch (rangedData.projectileData.projectileToSpawn.HitboxShape)
                            {
                                case HitboxShapeType.Circle:
                                    Gizmos.DrawWireSphere(singleProjEndPos, rangedData.projectileData.projectileToSpawn.CircleRadius);
                                    break;

                                case HitboxShapeType.Box:
                                    Gizmos.DrawWireMesh(debugCube, -1, singleProjEndPos, gameObject.transform.rotation * Quaternion.AngleAxis(rangedData.projectileData.angleOffset, transform.forward), rangedData.projectileData.projectileToSpawn.BoxSize);
                                    break;
                            }
                            Gizmos.DrawLine(projectileSpawnPos, singleProjEndPos);
                        }
                    }
                    break;

                case ActionType.ChargeAttack:
                    ChargeData chargeData = (ChargeData)actionConfig.data;

                    if (chargeData.showDebug && chargeData != null)
                    {
                        if(!Application.isPlaying || Application.isPlaying && mainBehaviour.FSM.CurrentState.Action.GetType() != typeof(ChargeAttack) && currentTarget)
                        {
                            Vector3 startPos = transform.position;
                            Vector3 targetPos = startPos + ((new Vector3(0, -1)) * chargeData.effectiveRange);
                            if(currentTarget)
                                targetPos = startPos + (currentTarget.transform.position - startPos).normalized * chargeData.effectiveRange;
                            Gizmos.color = chargeData.rangeColor;
                            Gizmos.DrawWireSphere(transform.position, chargeData.activationRange);
                            Gizmos.color = chargeData.startColor;
                            Gizmos.DrawWireSphere(startPos, circleCollider.radius);
                            Gizmos.color = chargeData.targetColor;
                            Gizmos.DrawWireSphere(targetPos, circleCollider.radius);
                            Gizmos.DrawLine(startPos, targetPos);
                        }
                    }
                    break;
            }
        }
    }
}
