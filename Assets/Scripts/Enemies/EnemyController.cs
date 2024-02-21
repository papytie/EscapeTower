using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(EnemyLifeSystem), typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyMovement), typeof(EnemyCollision))]

public class EnemyController : MonoBehaviour
{
    [Header("Behaviors")]
    [SerializeField] bool chaseBehavior = false;
    [SerializeField] bool fleeBehavior = false;
    [SerializeField] bool StayAtBehavior = false;
    [SerializeField] bool AutoAttack = false;

    GameObject playerObject;
    Animator animator;
    EnemyLifeSystem lifeSystem;
    EnemyAttack attack;
    EnemyMovement movement;
    EnemyCollision collision;
    EnemyEffectSystem effectSystem;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lifeSystem = GetComponent<EnemyLifeSystem>();
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();
        collision = GetComponent<EnemyCollision>();
        effectSystem = transform.GetComponentInChildren<EnemyEffectSystem>();

        playerObject = FindFirstObjectByType<PlayerController>().transform.gameObject;
        if(!playerObject)
        {
            Debug.Log("No Player target!");
            return;
        }
    }

    void Start()
    {
        lifeSystem.InitRef(animator);
        movement.SetTarget(playerObject, collision);
        attack.SetTarget(playerObject, effectSystem);
    }

    private void Update()
    {
        if (!attack.IsAttacking && !lifeSystem.IsDead)
        {

            if(movement.CanMove)
            {
                if (AutoAttack && !attack.IsOnCooldown)
                    attack.EnemyAttackActivation();

                movement.LookAtTarget();
            
                if (chaseBehavior)
                    movement.ChaseTarget();

                if (fleeBehavior)
                    movement.FleeTarget();

                if (StayAtBehavior)
                    movement.StayAtRangeFromTarget();

            }

        }

    }
}
