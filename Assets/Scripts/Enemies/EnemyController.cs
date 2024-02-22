using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    None,
    Chase,
    Flee,
    StayInRange
}

[RequireComponent(typeof(Animator), typeof(EnemyLifeSystem), typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyCollision), typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyAttack), typeof(EnemyChase), typeof(EnemyFlee))]
[RequireComponent(typeof(EnemyStayAtRange), typeof(EnemyBump))]

public class EnemyController : MonoBehaviour
{
    [Header("Behaviors")]
    [SerializeField] MovementType movementType;
    [SerializeField] bool AutoAttack = false;

    //Target Player Object
    GameObject targetObject = null;

    //Unity components
    Animator animator;

    //Enemy scripts
    EnemyLifeSystem lifeSystem;
    EnemyCollision collision;
    EnemyEffectSystem effectSystem;
    EnemyDetection detection;

    //Behavior scripts
    EnemyAttack attack;
    EnemyChase chase;
    EnemyFlee flee;
    EnemyStayAtRange stayAtRange;
    EnemyBump bump;

    private void Awake()
    {
        targetObject = null;

        animator = GetComponent<Animator>();

        lifeSystem = GetComponent<EnemyLifeSystem>();
        collision = GetComponent<EnemyCollision>();
        detection = GetComponent<EnemyDetection>();
        effectSystem = transform.GetComponentInChildren<EnemyEffectSystem>();
        
        attack = GetComponent<EnemyAttack>();
        chase = GetComponent<EnemyChase>();
        flee = GetComponent<EnemyFlee>();
        stayAtRange = GetComponent<EnemyStayAtRange>();
        bump = GetComponent<EnemyBump>();
    }

    void Start()
    {
        //Animator ref
        lifeSystem.InitRef(animator);

        //Effect ref
        attack.InitRef(effectSystem);

        //Collision ref
        chase.InitRef(collision);
        flee.InitRef(collision);
        stayAtRange.InitRef(collision);
        bump.InitRef(collision);
    }

    private void Update()
    {
        //Run detection until enemy find player
        if (detection.PlayerDetection(out GameObject target) && targetObject == null)
            targetObject = target;

        else targetObject = null;

        if (targetObject == null) return;

        //Check and use right behavior option
        if (!attack.IsAttacking && !lifeSystem.IsDead)
        {

            if(bump.CanMove)
            { 
                if (AutoAttack && !attack.IsOnCooldown)
                    attack.EnemyAttackActivation(targetObject);

                switch (movementType)
                {
                    case MovementType.None:
                        break;
                    case MovementType.Chase:
                        chase.ChaseTarget(targetObject);
                        break;
                    case MovementType.Flee:
                        flee.FleeTarget(targetObject);
                        break;
                    case MovementType.StayInRange:
                        stayAtRange.StayAtRangeFromTarget(targetObject);
                        break;

                }

            }

        }

    }

}

