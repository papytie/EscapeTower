using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheckerComponent), typeof(EnemyDetectionComponent), typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyStatsComponent), typeof(EnemyLifeSystemComponent), typeof(BumpComponent))]
[RequireComponent(typeof(EnemyLootSystem))]

public class EnemyController : MonoBehaviour
{
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
    bool InCurrentAttackRange => currentAttack != null && currentTarget != null && currentAttack.AttackRange <= Vector2.Distance(transform.position, currentTarget.transform.position); //Define a method to check attack Range in Ranged and Melee attack (IAttack)
    bool CanMove => !currentAttack.IsOnAttackLag;
    bool CanAttack => !currentAttack.IsAttacking && !currentAttack.IsOnCooldown;
    bool PlayerDetected => currentTarget != null;

    IMovement currentMovement;
    IAttack currentAttack;

    public Vector2 CurrentDirection => currentDirection;
    Vector2 currentDirection;

    [SerializeField] BehaviourConfig behaviourConfig;

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
        InstantiateBehaviour();
    }

    private void Start()
    {
        lifeSystem.InitRef(animator, lootSystem, bump, stats);
        bump.InitRef(collision);
        collision.Init();
        detection.InitRef(stats);
        behaviourConfig.behaviour.InitBehaviour();
    }

    private void Update()
    {
        if (lifeSystem.IsDead) return;

        //TODO : Switch for each state ? Priorize actions if currentAttack != null ? Think about it !
    }

/*        if (currentMovement != null)
        {
            currentMovement.Move(currentTarget, collision, circleCollider, stats.MoveSpeed);
            //Move Animation        
            currentDirection = currentMovement.EnemyDirection;
            animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.speed, 1 + MathF.Round(stats.MoveSpeed / 2, 1));
            animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.up, currentMovement.EnemyDirection.y);
            animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.right, currentMovement.EnemyDirection.x);
        }
*/    

    public void SetStatsScalingFactor(float value)
    {
        stats.SetScalingFactorTo(value);
    }

    void InstantiateBehaviour()
    {
        switch (behaviourConfig.behaviourType)
        {
            case BehaviourType.Knight:
                if (behaviourConfig.behaviour == null || behaviourConfig.behaviour != null && behaviourConfig.behaviour.GetType() != typeof(KnightBehaviour))
                behaviourConfig.behaviour = gameObject.AddComponent<KnightBehaviour>();
                break;
            case BehaviourType.Eye:
                if (behaviourConfig.behaviour == null || behaviourConfig.behaviour != null && behaviourConfig.behaviour.GetType() != typeof(KnightBehaviour))
                    behaviourConfig.behaviour = gameObject.AddComponent<KnightBehaviour>();
                break;
        }
    }

    private void OnValidate()
    {
        Type behaviourDataType = behaviourConfig.behaviourType switch
        {
            BehaviourType.Knight => typeof(KnightBehaviourData),
            BehaviourType.Eye => typeof(KnightBehaviourData),
            _ => null,
        };

        if (behaviourConfig.behaviourData == null && behaviourDataType != null || behaviourConfig.behaviourData != null && behaviourConfig.behaviourData.GetType() != behaviourDataType)
            behaviourConfig.behaviourData = BehaviourDataFactory.CreateBehaviourData(behaviourConfig.behaviourType);
    }

}