using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheckerComponent), typeof(EnemyDetectionComponent), typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyStatsComponent), typeof(EnemyLifeSystemComponent))] 
[RequireComponent(typeof(EnemyLootSystem), typeof(EnemyAnimationComponent))]

public class EnemyController : MonoBehaviour
{
    //Accessors for each components
    public EnemyStatsComponent Stats => stats;
    public EnemyLifeSystemComponent LifeSystem => lifeSystem;
    public EnemyDetectionComponent Detection => detection;
    public EnemyLootSystem LootSystem => lootSystem;
    public EnemyAnimationComponent AnimationParam => animationParam;
    public CollisionCheckerComponent Collision => collision;
    public Animator Animator => animator;
    public CircleCollider2D CircleCollider => circleCollider;

    public IBehaviour Behaviour => behaviour;
    IBehaviour behaviour;

    public GameObject CurrentTarget => currentTarget;
    GameObject currentTarget;

    public Vector2 CurrentDirection { get; set; }

    public bool InAttackRange => false; //Define a method to check attack Range in Ranged and Melee attack (IAttack)
    public bool TargetAcquired => currentTarget != null;
    
    [SerializeField] BehaviourConfig behaviourConfig;

    EnemyStatsComponent stats;
    EnemyLifeSystemComponent lifeSystem;
    EnemyDetectionComponent detection;
    EnemyLootSystem lootSystem;
    EnemyAnimationComponent animationParam;
    CollisionCheckerComponent collision;
    Animator animator;
    CircleCollider2D circleCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        circleCollider = GetComponent<CircleCollider2D>();
        stats = GetComponent<EnemyStatsComponent>();
        lifeSystem = GetComponent<EnemyLifeSystemComponent>();
        detection = GetComponent<EnemyDetectionComponent>();
        lootSystem = GetComponent<EnemyLootSystem>();
        animationParam = GetComponent<EnemyAnimationComponent>();
        collision = GetComponent<CollisionCheckerComponent>();

        InstantiateBehaviourComponents();
    }

    private void Start()
    {
        lifeSystem.InitRef(this);
        stats.Init();
        detection.InitRef(this);
        animationParam.InitRef(this);
        collision.Init();

        if (behaviourConfig != null)
        {
            behaviour.InitBehaviour(this);
        }
        else Debug.LogWarning("BehaviourConfig is missing on : " + gameObject.name);
    }

    private void Update()
    {
        if (behaviour == null) return;

        currentTarget = detection.PlayerDetection(out GameObject player) ? player : null;

        behaviour.FSM.CurrentState.Update();
    }

    public void SetStatsScalingFactor(float value)
    {
        stats.SetScalingFactorTo(value);
    }

    void InstantiateBehaviourComponents()
    {
        if (behaviourConfig == null) return;

        behaviour = BehaviourFactory.Create(gameObject, behaviourConfig.behaviourType);

        if (behaviour == null) return;

        behaviour.FSM = new NPCFSM();

        foreach (ActionConfig actionConfig in behaviourConfig.data.Actions)
        {
            IAction action = ActionFactory.Create(gameObject, actionConfig.ActionType);
            action.InitRef(actionConfig.data, this);
            behaviour.FSM.AddState(new NPCState(behaviour.FSM, actionConfig.ActionID, action));            
        }
        behaviour.InitBehaviour(this);
    }

    private void OnValidate()
    {
        //Get Base Components and set essentials references for Debug and accesiblity in editor mode
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        circleCollider = GetComponent<CircleCollider2D>();
        stats = GetComponent<EnemyStatsComponent>();
        lifeSystem = GetComponent<EnemyLifeSystemComponent>();
        detection = GetComponent<EnemyDetectionComponent>();
        lootSystem = GetComponent<EnemyLootSystem>();
        collision = GetComponent<CollisionCheckerComponent>();
        animationParam = GetComponent<EnemyAnimationComponent>();

        lifeSystem.InitRef(this);
        detection.InitRef(this);
        collision.Init();
        stats.Init();
        animationParam.InitRef(this);
    }

    private void OnDrawGizmos()
    {
        foreach (ActionConfig actionConfig in behaviourConfig.data.Actions)
        {
            switch (actionConfig.ActionType)
            {
                case ActionType.MeleeAttack:
                    MeleeData meleeData = (MeleeData)actionConfig.data;

                    if (meleeData.showDebug && meleeData != null)
                    {
                        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, new(0f,-1f));
                        if (Application.isPlaying)
                            currentRotation = Quaternion.LookRotation(Vector3.forward, behaviour.FSM.CurrentState.Action.Direction);

                        Vector3 center = transform.position.ToVector2() + circleCollider.offset;
                        Vector2 hitboxStartPos = center + currentRotation * meleeData.hitbox.startPosOffset;
                        Vector2 hitboxEndPos = center + currentRotation * meleeData.hitbox.endPos;

                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(center, 0.02f);

                        Gizmos.color = meleeData.debugColor;
                        Gizmos.DrawWireSphere(center, stats.MeleeRange);

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
                            currentRotation = Quaternion.LookRotation(Vector3.forward, behaviour.FSM.CurrentState.Action.Direction);

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
                                        Gizmos.DrawWireMesh(rangedData.debugCube, -1, multProjHitboxEndPos, currentRotation * angleRotation, rangedData.projectileData.projectileToSpawn.BoxSize);
                                        break;
                                }
                                Gizmos.DrawLine(projectileSpawnPos, multProjPos);
                            }
                        }
                        else
                        {
                            Vector3 singleProjEndPos = projectileSpawnPos + currentRotation * Vector3.up * rangedData.projectileData.range;

                            switch (rangedData.projectileData.projectileToSpawn.HitboxShape)
                            {
                                case HitboxShapeType.Circle:
                                    Gizmos.DrawWireSphere(singleProjEndPos, rangedData.projectileData.projectileToSpawn.CircleRadius);
                                    break;

                                case HitboxShapeType.Box:
                                    Gizmos.DrawWireMesh(rangedData.debugCube, -1, singleProjEndPos, gameObject.transform.rotation * Quaternion.AngleAxis(rangedData.projectileData.angleOffset, transform.forward), rangedData.projectileData.projectileToSpawn.BoxSize);
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
                        if(!Application.isPlaying || Application.isPlaying && behaviour.FSM.CurrentState.Action.GetType() != typeof(ChargeAttack) && currentTarget)
                        {
                            Vector3 startPos = transform.position;
                            Vector3 targetPos = startPos + ((new Vector3(0, -1)) * stats.MeleeRange*2);
                            if(currentTarget)
                                targetPos = startPos + (currentTarget.transform.position - startPos).normalized * stats.MeleeRange * 2;
                            Gizmos.color = chargeData.rangeColor;
                            Gizmos.DrawWireSphere(transform.position, stats.MeleeRange);
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
