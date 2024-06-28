using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheckerComponent), typeof(EnemyDetectionComponent), typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyStatsComponent), typeof(EnemyLifeSystemComponent), typeof(BumpComponent))]
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
    public BumpComponent Bump => bump;
    public Animator Animator => animator;
    public CircleCollider2D CircleCollider => circleCollider;
    
    EnemyStatsComponent stats;
    EnemyLifeSystemComponent lifeSystem;
    EnemyDetectionComponent detection;
    EnemyLootSystem lootSystem;
    EnemyAnimationComponent animationParam;

    CollisionCheckerComponent collision;
    BumpComponent bump;

    Animator animator;
    CircleCollider2D circleCollider;

    //Target stored and accessible
    public GameObject CurrentTarget => currentTarget;
    GameObject currentTarget;

    //TODO : Create accessors for each conditions of state change
    public bool InAttackRange => false; //Define a method to check attack Range in Ranged and Melee attack (IAttack)
    public bool TargetAcquired => currentTarget != null;

    public Vector2 CurrentDirection => currentDirection;

    IAction currentAction;
    Vector2 currentDirection;

    [SerializeField] BehaviourConfig behaviourConfig;

    public Dictionary<ActionStateType, IAction> EnemyActions => enemyActions;
    
    Dictionary<ActionStateType, IAction> enemyActions = new();

    IBehaviour enemyBehaviour;

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
        bump = GetComponent<BumpComponent>();

        InstantiateBehaviourComponents();
    }

    private void Start()
    {
        lifeSystem.InitRef(this);
        detection.InitRef(this);
        animationParam.InitRef(this);
        collision.Init();
        bump.InitRef(collision);

        if (behaviourConfig != null)
        {
            enemyBehaviour.InitBehaviour(this);
            //enemyBehaviour.InitFSM(this);
        }
        else Debug.LogWarning("BehaviourConfig is missing on : " + gameObject.name);
    }

    private void Update()
    {
        if (lifeSystem.IsDead || enemyBehaviour == null) return;

        currentTarget = detection.PlayerDetection(out GameObject player) ? player : null;
        
        enemyBehaviour.FSM.CurrentState.Update();
        //Debug.Log(enemyBehaviour.ToString() + (" ") + enemyBehaviour.FSM.ToString() + (" ") + enemyBehaviour.FSM.CurrentState.ToString());
    }

    public void SetStatsScalingFactor(float value)
    {
        stats.SetScalingFactorTo(value);
    }

    void InstantiateBehaviourComponents()
    {
        if (behaviourConfig == null) return;

        enemyBehaviour = behaviourConfig.behaviourType switch
        {
            BehaviourType.Stalker => gameObject.AddComponent<Stalker>(),
            BehaviourType.Harasser => gameObject.AddComponent<Harasser>(),
            BehaviourType.Harmless => gameObject.AddComponent<Harmless>(),
            _ => null,
        };

        if (behaviourConfig.Data.Actions.Count > 0) 
        { 
            foreach (ActionConfig actionConfig in behaviourConfig.Data.Actions)
            {
                switch (actionConfig.StateType)
                {
                    case ActionStateType.RoamMove:
                        if (!enemyActions.ContainsKey(ActionStateType.RoamMove))
                        {
                            RoamMove roam = gameObject.AddComponent<RoamMove>();
                            enemyActions.Add(ActionStateType.RoamMove, roam);
                            roam.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.ChaseMove:
                        if (!enemyActions.ContainsKey(ActionStateType.ChaseMove))
                        {
                            ChaseMove chase = gameObject.AddComponent<ChaseMove>();
                            enemyActions.Add(ActionStateType.ChaseMove, chase);
                            chase.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.FleeMove:
                        if (!enemyActions.ContainsKey(ActionStateType.FleeMove))
                        {
                            FleeMove flee = gameObject.AddComponent<FleeMove>();
                            enemyActions.Add(ActionStateType.FleeMove, flee);
                            flee.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.StayAtRangeMove:
                        if (!enemyActions.ContainsKey(ActionStateType.StayAtRangeMove))
                        {
                            StayAtRangeMove stayAtRange = gameObject.AddComponent<StayAtRangeMove>();
                            enemyActions.Add(ActionStateType.StayAtRangeMove, stayAtRange);
                            stayAtRange.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.TurnAroundMove:
                        if (!enemyActions.ContainsKey(ActionStateType.TurnAroundMove))
                        {
                            TurnAroundMove turnAround = gameObject.AddComponent<TurnAroundMove>();
                            enemyActions.Add(ActionStateType.TurnAroundMove, turnAround);
                            turnAround.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.WaitMove:
                        if (!enemyActions.ContainsKey(ActionStateType.WaitMove))
                        {
                            WaitAction wait = gameObject.AddComponent<WaitAction>();
                            enemyActions.Add(ActionStateType.WaitMove, wait);
                            wait.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.MeleeAttack:
                        if (!enemyActions.ContainsKey(ActionStateType.MeleeAttack))
                        {
                            MeleeAttack melee = gameObject.AddComponent<MeleeAttack>();
                            enemyActions.Add(ActionStateType.MeleeAttack, melee);
                            melee.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.RangedAttack:
                        if (!enemyActions.ContainsKey(ActionStateType.RangedAttack))
                        {
                            RangedAttack ranged = gameObject.AddComponent<RangedAttack>();
                            enemyActions.Add(ActionStateType.RangedAttack, ranged);
                            ranged.InitRef(actionConfig.data, this);
                        }
                        break;

                    case ActionStateType.ChargeAttack:
                        if (!enemyActions.ContainsKey(ActionStateType.ChargeAttack))
                        {
                            ChargeAttack charge = gameObject.AddComponent<ChargeAttack>();
                            enemyActions.Add(ActionStateType.ChargeAttack, charge);
                            charge.InitRef(actionConfig.data, this);
                        }
                        break;
                }
            }
        }
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
        bump = GetComponent<BumpComponent>();

        lifeSystem.InitRef(this);
        detection.InitRef(this);
        collision.Init();
        bump.InitRef(collision);
    }
}
