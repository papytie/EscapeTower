using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCollisionComponent), typeof(Animator))]
[RequireComponent(typeof(EnemyAttack), typeof(EnemyLifeSystemComponent), typeof(EnemyBumpComponent))]

public class EnemyController : MonoBehaviour
{
    [Header("Settings"), Space]
    [SerializeField] GameObject player;
    [SerializeField] List<MovementConfig> configList = new();

    // TODO : create a new Dictionnary<string, IAttackFX> AttackFXBehaviors

    [SerializeField] List<EnemyAttackData> attackList = new();
    [SerializeField] float timerDuration = 5;

    Dictionary<MovementType, IMovement> movementBehaviors = new();

    EnemyCollisionComponent collision;
    EnemyLifeSystemComponent lifeSystem;
    EnemyBumpComponent bump;
    Animator animator;
    EnemyAttack attack;
    int currentAttackIndex = 0;
    int currentMovementIndex = 0;
    float startTime = 0;

    IMovement currentMovement;

    private void Awake()
    {
        attack = GetComponent<EnemyAttack>();
        collision = GetComponent<EnemyCollisionComponent>();
        animator = GetComponent<Animator>();
        lifeSystem = GetComponent<EnemyLifeSystemComponent>();
        bump = GetComponent<EnemyBumpComponent>();
        InitMovementBehaviors();
    }

    private void Start()
    {
        SetMovementConfig();
        attack.InitRef(animator);
        lifeSystem.InitRef(animator);
        bump.InitRef(collision);
    }

    private void Update()
    {
        if (lifeSystem.IsDead) return;

        if (configList[currentMovementIndex].type != MovementType.Wait)
            currentMovement.Move(player, collision);

        if(Time.time > startTime + timerDuration)
        {
            //TODO : fix the init
            //attack.InitAttackData(attackList[currentAttackIndex]);
            attack.AttackActivation();

            currentMovementIndex = currentMovementIndex >= configList.Count-1 ? 0 : currentMovementIndex + 1;
            currentAttackIndex = currentAttackIndex >= attackList.Count-1 ? 0 : currentAttackIndex + 1;
            startTime = Time.time;
            
            SetMovementConfig();

        }

    }

    void SetMovementConfig()
    {
        if (configList[currentMovementIndex].type == MovementType.Wait) return;

        currentMovement = movementBehaviors[configList[currentMovementIndex].type];
        currentMovement.Init(configList[currentMovementIndex].data);
    }

    void InitMovementBehaviors()
    {
        foreach (MovementConfig script in configList) 
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
                    if(!movementBehaviors.ContainsKey(MovementType.StayAtRange))
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

    private void OnValidate()
    {
        foreach (var item in configList)
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