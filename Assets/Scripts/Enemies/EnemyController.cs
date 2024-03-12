using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCollision))]

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] List<MovementConfig> configs = new List<MovementConfig>();

    EnemyCollision collision;
    IMovement currentMovement;

    private void Start()
    {
        collision = GetComponent<EnemyCollision>();
    }

    /*IMovement InitMovement(MovementType type)
    {
        return type switch
        {
            MovementType.Wait => null,
            MovementType.Chase => chase,
            MovementType.Flee => flee,
            MovementType.StayAtRange => stayAtRange,
            _ => null,
        };
    }*/

    private void Update()
    {
        currentMovement.Move(player, collision);
    }

    private void OnValidate()
    {
        foreach (var item in configs)
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