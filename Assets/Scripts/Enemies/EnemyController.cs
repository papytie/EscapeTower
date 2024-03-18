using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCollision), typeof(Animator), typeof(EnemyEffectSystem))]
[RequireComponent(typeof(Attack))]

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] List<MovementConfig> configList = new();
    [SerializeField] int currentIndex = 0;
    [SerializeField] float startTime = 0;
    [SerializeField] float timerDuration = 5;

    Dictionary<MovementType, IMovement> movementBehaviors = new();

    EnemyCollision collision;
    EnemyEffectSystem effects;
    Animator animator;
    Attack attack;
    IMovement currentMovement;

    private void Awake()
    {
        attack = GetComponent<Attack>();
        collision = GetComponent<EnemyCollision>();
        animator = GetComponent<Animator>();
        effects = GetComponent<EnemyEffectSystem>();
        InitMovementBehaviors();
    }

    private void Start()
    {
        SetMovementConfig();
        attack.InitRef(effects, animator);
    }

    private void Update()
    {
        if (configList[currentIndex].type != MovementType.Wait)
            currentMovement.Move(player, collision);

        if(Time.time > startTime + timerDuration)
        {
            //TODO:
            attack.AttackActivation();

            currentIndex = currentIndex >= configList.Count-1 ? 0 : currentIndex + 1;
            startTime = Time.time;
            
            SetMovementConfig();

        }

    }

    void SetMovementConfig()
    {
        if (configList[currentIndex].type == MovementType.Wait) return;

        currentMovement = movementBehaviors[configList[currentIndex].type];
        currentMovement.Init(configList[currentIndex].data);
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