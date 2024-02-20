using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(EnemyLifeSystem), typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyMovement))]

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] GameObject playerObject;

    [Header("Behaviors")]
    [SerializeField] bool chaseBehavior = false;
    [SerializeField] bool fleeBehavior = false;
    [SerializeField] bool StayAtBehavior = false;
    [SerializeField] bool AutoAttack = false;

    Animator animator;
    EnemyLifeSystem lifeSystem;
    EnemyAttack attack;
    EnemyMovement movement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lifeSystem = GetComponent<EnemyLifeSystem>();
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();

        playerObject = FindFirstObjectByType<PlayerController>().transform.gameObject;
        if(!playerObject)
        {
            Debug.Log("No target!");
            return;
        }
    }

    void Start()
    {
        lifeSystem.InitRef(animator);
        movement.SetTarget(playerObject);
        attack.SetTarget(playerObject);
    }

    private void Update()
    {
        if (attack.AttackAvailable && AutoAttack)
            attack.EnemyAttackActivation();

        if (chaseBehavior)
            movement.ChaseTarget();

        if (fleeBehavior)
            movement.FleeTarget();

        if (StayAtBehavior)
            movement.StayAtRangeFromTarget();

    }
}
