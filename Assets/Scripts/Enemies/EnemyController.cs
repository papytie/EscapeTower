using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCollision))]

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] List<MovementConfig> configList = new();
    [SerializeField] int currentIndex = 0;
    [SerializeField] float startTime = 0;
    [SerializeField] float timerDuration = 5;

    Dictionary<MovementType, IMovement> movementBehaviors = new();

    EnemyCollision collision;
    IMovement currentMovement;

    private void Awake()
    {
        collision = GetComponent<EnemyCollision>();
        InitMovementBehaviors();
        
    }

    private void Start()
    {
        SetMovementConfig();
    }

    private void Update()
    {
        currentMovement.Move(player, collision);

        if(Time.time > startTime + timerDuration)
        {
            currentIndex = currentIndex >= configList.Count-1 ? 0 : currentIndex + 1;
            startTime = Time.time;

            SetMovementConfig();
        }

    }

    void SetMovementConfig()
    {
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
                _ => null,
            };

            if ((item.data == null && dataType != null) || (item.data != null && item.data.GetType() != dataType))
            {
                item.data = MovementDataFactory.CreateData(item.type);
            }

        }

    }

}