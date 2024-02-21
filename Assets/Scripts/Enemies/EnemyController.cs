using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(EnemyLifeSystem), typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyMovement), typeof(EnemyCollision), typeof(EnemyDetection))]

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
    EnemyDetection detection;

    private void Awake()
    {
        playerObject = null;
        animator = GetComponent<Animator>();
        lifeSystem = GetComponent<EnemyLifeSystem>();
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();
        collision = GetComponent<EnemyCollision>();
        detection = GetComponent<EnemyDetection>();
        effectSystem = transform.GetComponentInChildren<EnemyEffectSystem>();
        
    }

    void Start()
    {
        lifeSystem.InitRef(animator);
        movement.InitRef(collision);
        attack.InitRef(effectSystem);
    }

    private void Update()
    {
        //Run detection until enemy find player
        if(detection.PlayerDetection(out GameObject target) && playerObject == null)
        {
            playerObject = target;
            movement.SetTarget(target);
            attack.SetTarget(target);
        }

        //Check and use right behavior option
        if (!attack.IsAttacking && !lifeSystem.IsDead && playerObject != null)
        {

            if(movement.CanMove)
            {
                movement.LookAtTarget();

                if (AutoAttack && !attack.IsOnCooldown)
                    attack.EnemyAttackActivation();
            
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
