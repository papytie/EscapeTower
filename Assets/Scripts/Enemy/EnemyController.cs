using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCollisionComponent), typeof(Animator))]
[RequireComponent(typeof(EnemyAttackComponent), typeof(EnemyLifeSystemComponent), typeof(EnemyBumpComponent))]

public class EnemyController : MonoBehaviour
{
    [Header("Settings"), Space]
    [SerializeField] GameObject player;
    [SerializeField] List<MovementConfig> movementList = new();
    [SerializeField] List<EnemyAttackConfig> attackList = new();
    [SerializeField] float timerDuration = 5;

    Dictionary<MovementType, IMovement> movementBehaviors = new();
    Dictionary<string, IAttackFX> attackFXBehaviors = new();

    EnemyCollisionComponent collision;
    EnemyLifeSystemComponent lifeSystem;
    EnemyBumpComponent bump;
    Animator animator;
    EnemyAttackComponent attack;
    int currentAttackIndex = 0;
    int currentMovementIndex = 0;
    float startTime = 0;

    IMovement currentMovement;

    public Vector2 CurrentDirection => currentDirection;
    Vector2 currentDirection;

    private void Awake()
    {
        attack = GetComponent<EnemyAttackComponent>();
        collision = GetComponent<EnemyCollisionComponent>();
        animator = GetComponent<Animator>();
        lifeSystem = GetComponent<EnemyLifeSystemComponent>();
        bump = GetComponent<EnemyBumpComponent>();
        InitMovementBehaviors();
        InitAttackFXBehaviors();
    }

    private void Start()
    {
        SetMovementConfig();
        attack.InitRef(animator, this);
        lifeSystem.InitRef(animator);
        bump.InitRef(collision);
    }

    private void Update()
    {
        if (lifeSystem.IsDead) return;

        currentDirection = currentMovement.EnemyDirection;
        animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.up, currentMovement.EnemyDirection.y);
        animator.SetFloat(SRAnimators.EnemyBaseAnimator.Parameters.right, currentMovement.EnemyDirection.x);

        if (movementList[currentMovementIndex].type != MovementType.Wait)
            currentMovement.Move(player, collision);

        if(Time.time > startTime + timerDuration)
        {
            if (attackList.Count == 0) return;
            
            attack.InitAttack(attackList[currentAttackIndex].attackData, attackFXBehaviors[attackList[currentAttackIndex].attackFXPrefab.name]);
            attack.AttackActivation();

            currentMovementIndex = currentMovementIndex >= movementList.Count-1 ? 0 : currentMovementIndex + 1;
            currentAttackIndex = currentAttackIndex >= attackList.Count-1 ? 0 : currentAttackIndex + 1;
            startTime = Time.time;
            
            SetMovementConfig();

        }

    }

    void SetMovementConfig()
    {
        if (movementList[currentMovementIndex].type == MovementType.Wait) return;

        currentMovement = movementBehaviors[movementList[currentMovementIndex].type];
        currentMovement.Init(movementList[currentMovementIndex].data);
    }

    void InitMovementBehaviors()
    {
        foreach (MovementConfig script in movementList) 
        {
            switch (script.type)
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

    void InitAttackFXBehaviors()
    {
        foreach (EnemyAttackConfig attackConfig in attackList)
        {
            if (attackConfig.attackFXPrefab == null)
            {
                Debug.LogWarning(attackConfig.name + " do not contain any FXPrefab");
            }

            if(!attackFXBehaviors.ContainsKey(attackConfig.attackFXPrefab.name))
            {
                IAttackFX attackFX = Instantiate(attackConfig.attackFXPrefab, transform).GetComponent<IAttackFX>();
                attackFXBehaviors.Add(attackConfig.attackFXPrefab.name, attackFX);
            }
            
        }
    }

    private void OnValidate()
    {
        foreach (var item in movementList)
        {
            Type dataType = item.type switch
            {
                MovementType.Wait => null,
                MovementType.Chase => typeof(ChaseData),
                MovementType.Flee => typeof(FleeData),
                MovementType.StayAtRange => typeof(StayAtRangeData),
                MovementType.TurnAround => typeof(TurnAroundData),
                _ => null,
            };

            if ((item.data == null && dataType != null) || (item.data != null && item.data.GetType() != dataType))
            {
                item.data = MovementDataFactory.CreateData(item.type);
            }

        }

    }

}