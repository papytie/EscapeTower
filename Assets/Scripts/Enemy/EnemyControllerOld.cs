/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheckerComponent), typeof(EnemyDetectionComponent), typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyStatsComponent), typeof(EnemyLifeSystemComponent), typeof(BumpComponent))]
[RequireComponent(typeof(EnemyLootSystem))]

public class EnemyControllerOld : MonoBehaviour
{
    [Header("Settings"), Space]
    [SerializeField] List<MovementConfig> movementList = new();
    [SerializeField] List<AttackConfig> attackList = new();
    [SerializeField] float refreshBehaviorTime = 5;

    Dictionary<MovementType, IMovement> movementBehaviors = new();
    Dictionary<AttackType, IAttack> attackBehaviors = new();
    Dictionary<string, IAttackFX> attackFXBehaviors = new();

    EnemyChase chaseMovement;
    EnemyFlee fleeMovement;
    EnemyStayAtRange stayAtRangeMovement;
    EnemyTurnAround turnAroundMovement;

    EnemyStatsComponent stats;
    EnemyLifeSystemComponent lifeSystem;
    EnemyDetectionComponent detection;
    EnemyLootSystem lootSystem;
    CollisionCheckerComponent collision;
    BumpComponent bump;
    Animator animator;
    CircleCollider2D circleCollider;
    GameObject currentTarget;

    //int currentAttackIndex = 0;
    //int currentMovementIndex = 0;

    float startTime = 0;

    StateType currentState;
    bool InCurrentAttackRange => currentAttack != null && currentTarget != null && currentAttack.AttackRange <= Vector2.Distance(transform.position, currentTarget.transform.position); //Define a method to check attack Range in Ranged and Melee attack (IAttack)
    bool CanMove => !currentAttack.IsOnAttackLag;
    bool CanAttack => !currentAttack.IsAttacking && !currentAttack.IsOnCooldown;
    bool PlayerDetected => currentTarget != null;

    IMovement currentMovement;
    IAttack currentAttack;

    public Vector2 CurrentDirection => currentDirection;
    Vector2 currentDirection;

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
        InitMovementBehaviors();
        InitAttackBehaviors();
    }

    private void Start()
    {
        lifeSystem.InitRef(animator, lootSystem, bump, stats);
        bump.InitRef(collision);
        collision.Init();
        detection.InitRef(stats);
    }

    private void Update()
    {
        if (lifeSystem.IsDead) return;

        if(Time.time > startTime + refreshBehaviorTime)
        {
            //Assign currentTarget value is detection is valid
            currentTarget = detection.PlayerDetection(out GameObject player) ? player : null;

            //Current attack selection in attackList 
            currentAttack = AttackSelection();

            //Refresh current state depending on detection status

            currentState = StateSelection();

            //currentMovement = 

            //Refresh timer
            startTime = Time.time;

            //Debug
            Debug.Log("Current state is : " + currentState);
        }

        //Main switch for enemy behavior
        switch (currentState)
        {
            case StateType.Wait:
                break;
            case StateType.SearchMove: 
                break;
            case StateType.ChaseMove:
                break;
            case StateType.StayAtRangeMove:
                break;
            case StateType.MeleeAttack:
                break;
            case StateType.FleeMove:
                break;
        }

        //Movement Process
        if (currentMovement != null)
        {
            currentMovement.Move(currentTarget, collision, circleCollider, stats.MoveSpeed);
            //Move Animation        
            currentDirection = currentMovement.EnemyDirection;
            animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.speed, 1 + MathF.Round(stats.MoveSpeed / 2, 1));
            animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.up, currentMovement.EnemyDirection.y);
            animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.right, currentMovement.EnemyDirection.x);
        }
    }

    IAttack AttackSelection()
    {
        if (currentTarget == null) return null;
        float targetDistance = Vector2.Distance(transform.position, currentTarget.transform.position);

        foreach (var attack in attackBehaviors)
        {
            if (attack.Value.AttackRange < targetDistance && !attack.Value.IsOnCooldown)
            {
                if (attack.Key == AttackType.Melee) 
                    return attack.Value;                  
                else return attack.Value;
            }
        }
        Debug.Log("No Valid Attack in attackBehaviors list !");
        return null;
    }

    StateType StateSelection()
    {
        if (currentTarget != null)
        {
            if (currentAttack != null && CanAttack)
                return StateType.MeleeAttack;

            else if (CanMove)
            {
                if (InCurrentAttackRange) 
                    return StateType.StayAtRangeMove;

                else return StateType.ChaseMove;
            }
            else return StateType.Wait;
        }
        else if (CanMove)
            return StateType.SearchMove;

        else return StateType.Wait;
    }

    public void SetStatsScalingFactor(float value)
    {
        stats.SetScalingFactorTo(value);
    }

    void InitMovementBehaviors()
    {
        foreach (MovementConfig moveConfig in movementList) 
        {
            switch (moveConfig.type)
            {
                case MovementType.Flee:
                    if(!movementBehaviors.ContainsKey(MovementType.Flee))
                    {
                        EnemyFlee flee = gameObject.AddComponent<EnemyFlee>();
                        movementBehaviors.Add(MovementType.Flee, flee);
                    }
                    break;

                case MovementType.Chase:
                    if (!movementBehaviors.ContainsKey(MovementType.Chase))
                    {
                        EnemyChase chase = gameObject.AddComponent<EnemyChase>();
                        movementBehaviors.Add(MovementType.Chase, chase);
                    }
                    break;

                case MovementType.StayAtRange:
                    if(!movementBehaviors.ContainsKey(MovementType.StayAtRange))
                    {
                        EnemyStayAtRange stayAtRange = gameObject.AddComponent<EnemyStayAtRange>();
                        movementBehaviors.Add(MovementType.StayAtRange, stayAtRange);
                    }
                    break;

                case MovementType.TurnAround:
                    if(!movementBehaviors.ContainsKey(MovementType.TurnAround))
                    {
                        EnemyTurnAround turnAround = gameObject.AddComponent<EnemyTurnAround>();
                        movementBehaviors.Add(MovementType.TurnAround, turnAround);
                    }
                    break;

                case MovementType.Wait:
                    break;
            }
        }
    }

    void InitAttackBehaviors()
    {
        foreach (AttackConfig attackConfig in attackList)
        {
            switch (attackConfig.type)
            {
                case AttackType.Melee:
                    if(!attackBehaviors.ContainsKey(AttackType.Melee))
                    {
                        EnemyMeleeAttack meleeAttack = gameObject.AddComponent<EnemyMeleeAttack>();
                        attackBehaviors.Add(AttackType.Melee, meleeAttack);
                    }
                    break;

                case AttackType.Ranged:
                    if (!attackBehaviors.ContainsKey(AttackType.Ranged))
                    {
                        EnemyRangedAttack rangedAttack = gameObject.AddComponent<EnemyRangedAttack>();
                        attackBehaviors.Add(AttackType.Ranged, rangedAttack);
                    }
                    break;
            }

            if (attackConfig.attackFXPrefab == null)
            {
                Debug.LogWarning(attackConfig + " do not contain any FXPrefab");
                return;
            }

            if (!attackFXBehaviors.ContainsKey(attackConfig.attackFXPrefab.name))
            {
                IAttackFX attackFX = Instantiate(attackConfig.attackFXPrefab, transform).GetComponent<IAttackFX>();
                attackFXBehaviors.Add(attackConfig.attackFXPrefab.name, attackFX);
                Debug.Log(attackConfig.attackFXPrefab + " have been successfully instantied !");
            }
            
        }
    }

    private void OnValidate()
    {
        foreach (MovementConfig movementConfig in movementList)
        {
            Type movementType = movementConfig.type switch
            {
                MovementType.Wait => null,
                MovementType.Chase => typeof(ChaseData),
                MovementType.Flee => typeof(FleeData),
                MovementType.StayAtRange => typeof(StayAtRangeData),
                MovementType.TurnAround => typeof(TurnAroundData),
                _ => null,
            };

            if ((movementConfig.data == null && movementType != null) || (movementConfig.data != null && movementConfig.data.GetType() != movementType))
            {
                movementConfig.data = MovementDataFactory.CreateData(movementConfig.type);
            }
        }

        foreach (AttackConfig attackConfig in attackList)
        {
            Type attackType = attackConfig.type switch
            {
                AttackType.Melee => typeof(MeleeAttackData),
                AttackType.Ranged => typeof(RangedAttackData),
                _ => null,
            };

            if ((attackConfig.attackData == null && attackType != null) || (attackConfig.attackData != null && attackConfig.attackData.GetType() != attackType))
            {
                attackConfig.attackData = AttackDataFactory.CreateData(attackConfig.type);
            }
        }
    }
}*/