using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheckerComponent), typeof(EnemyDetectionComponent), typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyStatsComponent), typeof(EnemyLifeSystemComponent), typeof(BumpComponent))]
[RequireComponent(typeof(EnemyLootSystem))]

public class EnemyController : MonoBehaviour
{
    public CollisionCheckerComponent Collision => collision;
    public CircleCollider2D CircleCollider => circleCollider;
    public EnemyStatsComponent Stats => stats;
    public Animator Animator => animator;
    public GameObject CurrentTarget => currentTarget;
    
    EnemyStatsComponent stats;
    EnemyLifeSystemComponent lifeSystem;
    EnemyDetectionComponent detection;
    EnemyLootSystem lootSystem;
    CollisionCheckerComponent collision;
    BumpComponent bump;
    Animator animator;
    CircleCollider2D circleCollider;
    GameObject currentTarget;

    float startTime = 0;

    StateType currentState;

    //TODO : Create accessors for each conditions of state change
    public bool InAttackRange => false; //Define a method to check attack Range in Ranged and Melee attack (IAttack)
    bool CanMove => !currentAttack.IsOnAttackLag;
    public bool CanAttack => !currentAttack.IsAttacking && !currentAttack.IsOnCooldown;
    public bool AttackOnCD => currentAttack.IsOnCooldown;
    public bool TargetAcquired => currentTarget != null;

    public IMovement CurrentMovement { get => currentMovement; set { currentMovement = value; } }
    public IAttack CurrentAttack { get => currentAttack; set { currentAttack = value; } }
    public Vector2 CurrentDirection => currentDirection;
    public Dictionary<MovementType, IMovement> EnemyMoves => enemyMoves;
    public Dictionary<AttackType, IAttack> EnemyAttacks => enemyAttacks;

    IMovement currentMovement;
    IAttack currentAttack;
    Vector2 currentDirection;

    //-----------------------------------------------\
    //--------------------BEHAVIOUR------------------|
    //-----------------------------------------------/

    Dictionary<MovementType, IMovement> enemyMoves = new();
    Dictionary<AttackType, IAttack> enemyAttacks = new();

    [SerializeField] BehaviourConfig behaviourConfig = new();
    IBehaviour currentBehaviour;

    private void Awake()
    {
        stats = GetComponent<EnemyStatsComponent>(); 
        collision = GetComponent<CollisionCheckerComponent>();

        animator = GetComponent<Animator>();
        if(animator == null)
            animator = GetComponentInChildren<Animator>();

        lifeSystem = GetComponent<EnemyLifeSystemComponent>();
        bump = GetComponent<BumpComponent>();
        detection = GetComponent<EnemyDetectionComponent>();
        lootSystem = GetComponent<EnemyLootSystem>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        lifeSystem.InitRef(animator, lootSystem, bump, stats);
        bump.InitRef(collision);
        collision.Init();
        detection.InitRef(stats);
        InstantiateBehaviourComponents();
        currentBehaviour.InitBehaviour(behaviourConfig.behaviourData, this);
    }

    private void Update()
    {
        if (lifeSystem.IsDead) return;

        //currentBehaviour.FSM.CurrentState.Update();

        /*switch (behaviourConfig.behaviour.CurrentState)
        {
            case StateType.Wait:
                break;
            case StateType.Roam:
                break;
            case StateType.Chase:
                break;
            case StateType.Charge:
                break;
        }*/

    }

    public void UpdateMoveAnimDirection(Vector2 direction)
    {
        animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.up, direction.y);
        animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.right, direction.x);
    }

    public void UpdateMoveAnimSpeed(float speed)
    {
        animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.speed, 1 + MathF.Round(speed / 2, 2));
    }
    

    public void SetStatsScalingFactor(float value)
    {
        stats.SetScalingFactorTo(value);
    }

    void InstantiateBehaviourComponents()
    {
        switch (behaviourConfig.behaviourType)
        {
            case BehaviourType.Knight:
                currentBehaviour = gameObject.AddComponent<KnightBehaviour>();
                break;
            case BehaviourType.Eye:
                currentBehaviour = gameObject.AddComponent<EyeBehaviour>();
                break;
        }

        if (behaviourConfig.behaviourData.MovesList.Count > 0) 
        { 
            foreach (MovementConfig moveConfig in behaviourConfig.behaviourData.MovesList)
            {
                switch (moveConfig.MoveType)
                {
                    case MovementType.Roam:
                        if (!enemyMoves.ContainsKey(MovementType.Roam))
                        {
                            EnemyRoam roam = gameObject.AddComponent<EnemyRoam>();
                            enemyMoves.Add(MovementType.Roam, roam);
                            roam.InitRef(moveConfig.data, this);
                        }
                        break;

                    case MovementType.Chase:
                        if (!enemyMoves.ContainsKey(MovementType.Chase))
                        {
                            EnemyChase chase = gameObject.AddComponent<EnemyChase>();
                            enemyMoves.Add(MovementType.Chase, chase);
                            chase.InitRef(moveConfig.data, this);
                        }
                        break;

                    case MovementType.Flee:
                        if (!enemyMoves.ContainsKey(MovementType.Flee))
                        {
                            EnemyFlee flee = gameObject.AddComponent<EnemyFlee>();
                            enemyMoves.Add(MovementType.Flee, flee);
                            flee.InitRef(moveConfig.data, this);
                        }
                        break;

                    case MovementType.StayAtRange:
                        if (!enemyMoves.ContainsKey(MovementType.StayAtRange))
                        {
                            EnemyStayAtRange stayAtRange = gameObject.AddComponent<EnemyStayAtRange>();
                            enemyMoves.Add(MovementType.StayAtRange, stayAtRange);
                            stayAtRange.InitRef(moveConfig.data, this);
                        }
                        break;

                    case MovementType.TurnAround:
                        if (!enemyMoves.ContainsKey(MovementType.TurnAround))
                        {
                            EnemyTurnAround turnAround = gameObject.AddComponent<EnemyTurnAround>();
                            enemyMoves.Add(MovementType.TurnAround, turnAround);
                            turnAround.InitRef(moveConfig.data, this);
                        }
                        break;

                    case MovementType.Wait:
                        break;
                }
            }
        }

        if (behaviourConfig.behaviourData.AttacksList.Count > 0)
        {
            foreach (AttackConfig attackConfig in behaviourConfig.behaviourData.AttacksList)
            {
                switch (attackConfig.AttackType)
                {
                    case AttackType.Melee:
                        if (!enemyAttacks.ContainsKey(AttackType.Melee))
                        {
                            EnemyMeleeAttack meleeAttack = gameObject.AddComponent<EnemyMeleeAttack>();
                            enemyAttacks.Add(AttackType.Melee, meleeAttack);
                        }
                        break;

                    case AttackType.Ranged:
                        if (!enemyAttacks.ContainsKey(AttackType.Ranged))
                        {
                            EnemyRangedAttack rangedAttack = gameObject.AddComponent<EnemyRangedAttack>();
                            enemyAttacks.Add(AttackType.Ranged, rangedAttack);
                        }
                        break;
                    case AttackType.Charge:
                        if (!enemyAttacks.ContainsKey(AttackType.Charge))
                        {
                            EnemyChargeAttack rangedAttack = gameObject.AddComponent<EnemyChargeAttack>();
                            enemyAttacks.Add(AttackType.Charge, rangedAttack);
                        }
                        break;


                }

                //FX----------------------------------------

                /*if (attackConfig.attackFXPrefab == null)
                {
                    Debug.LogWarning(attackConfig + " do not contain any FXPrefab");
                    return;
                }

                if (!attackFXBehaviors.ContainsKey(attackConfig.attackFXPrefab.name))
                {
                    IAttackFX attackFX = Instantiate(attackConfig.attackFXPrefab, transform).GetComponent<IAttackFX>();
                    attackFXBehaviors.Add(attackConfig.attackFXPrefab.name, attackFX);
                    Debug.Log(attackConfig.attackFXPrefab + " have been successfully instantied !");
                }*/
            }
        }
    }

    public Type GetType(KeyValuePair<MovementType, IMovement> moveKeyValue)
    {
        return moveKeyValue.Key switch
        {
            MovementType.Wait => null,
            MovementType.Chase => typeof(EnemyChase),
            MovementType.Flee => typeof(EnemyFlee),
            MovementType.StayAtRange => typeof(EnemyStayAtRange),
            MovementType.TurnAround => typeof(EnemyTurnAround),
            MovementType.Roam => typeof(EnemyRoam),
            _ => null,
        };
    }

    private void OnValidate()
    {
        Type behaviourType = behaviourConfig.behaviourType switch
        {
            BehaviourType.Knight => typeof(KnightBehaviourData),
            BehaviourType.Eye => typeof(EyeBehaviourData),
            _ => null,
        };

        if ((behaviourConfig.behaviourData == null && behaviourType != null) || (behaviourConfig.behaviourData != null && behaviourConfig.behaviourData.GetType() != behaviourType))
        {
            behaviourConfig.behaviourData = BehaviourDataFactory.CreateBehaviourData(behaviourConfig.behaviourType);
            behaviourConfig.behaviourData.InitConfigList();

            foreach (MovementConfig move in behaviourConfig.behaviourData.MovesList)
            {
                move.data = MovementDataFactory.CreateData(move.MoveType);
            }

            foreach (AttackConfig attack in behaviourConfig.behaviourData.AttacksList)
            {
                attack.data = AttackDataFactory.CreateData(attack.AttackType);
            }
        }
    }
}
