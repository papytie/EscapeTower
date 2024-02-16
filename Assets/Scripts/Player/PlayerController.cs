using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputs), typeof(PlayerMovement), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerWeaponSlot), typeof(PlayerAttack), typeof(Animator))]
[RequireComponent(typeof(PlayerLifeSystem), typeof(PlayerCollision))]

public class PlayerController : MonoBehaviour
{
    public bool CanMove => !dash.IsDashing && !attack.IsAttacking && !lifeSystem.IsDead;
    public bool CanDash => dash.DashAvailable && !lifeSystem.IsDead;
    public bool CanAttack => attack.AttackAvailable && !dash.IsDashing && !lifeSystem.IsDead;
    public bool CanTakeDamage => !lifeSystem.IsInvincible && !lifeSystem.IsDead;

    PlayerInputs inputs;
    PlayerMovement movement;
    PlayerDash dash;
    PlayerAttack attack;
    PlayerWeaponSlot slot;
    PlayerLifeSystem lifeSystem;
    PlayerCollision collision;
    Animator animator;

    private void Awake()
    {
        GetComponentsRef();  
    }

    void GetComponentsRef()
    {
        inputs = GetComponent<PlayerInputs>();
        movement = GetComponent<PlayerMovement>();
        dash = GetComponent<PlayerDash>();
        attack = GetComponent<PlayerAttack>();
        slot = GetComponent<PlayerWeaponSlot>();
        lifeSystem = GetComponent<PlayerLifeSystem>();
        collision = GetComponent<PlayerCollision>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        InitComponentsRef();
    }

    void InitComponentsRef()
    {
        movement.InitRef(inputs, animator, collision);
        dash.InitRef(collision, lifeSystem);
        attack.InitRef(inputs, slot);
        lifeSystem.InitRef(animator);
    }

    void Update()
    {
        Vector3 moveAxis = inputs.MoveAxisInput.ReadValue<Vector2>();
        Vector3 attackAxis = inputs.AttackAxisInput.ReadValue<Vector2>();

        //Movement
        if (CanMove && moveAxis != Vector3.zero) 
        {      
            movement.CheckedMove(moveAxis);

            //Movement rotation
            if (attackAxis == Vector3.zero && !inputs.AttackButtonInput.IsPressed())
                movement.RotateToMoveDirection(moveAxis);
        }        

        //Keyboard Attack
        if (CanAttack && inputs.AttackButtonInput.IsPressed())
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(inputs.MousePositionAxisInput.ReadValue<Vector2>());
            attack.AttackActivation((mouseWorldPos - transform.position).normalized);
            movement.RotateToMoveDirection((mouseWorldPos - transform.position).normalized);
        }

        //Gamepad Attack
        if (!lifeSystem.IsDead && attackAxis != Vector3.zero)
        {
            movement.RotateToMoveDirection(attackAxis);

            //Attack rotation
            if (CanAttack && attack.AutoAttackOnStick)
                 attack.AttackActivation(attackAxis);             
        }

        //Dash
        if (CanDash && inputs.DashButtonInput.WasPerformedThisFrame())
        {
            if (moveAxis != Vector3.zero)
                dash.DashActivation(moveAxis);
            else 
                dash.DashActivation(transform.up);
        }

        //Collision Check reaction
        if (CanTakeDamage)
        {
            if (collision.EnemyCheckCollision(collision.EnemyLayer, out int dmg))
            {
                lifeSystem.TakeDamage(dmg);
            }
        }
    }

}
