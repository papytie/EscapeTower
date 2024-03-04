using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PlayerInputs), typeof(PlayerMovement), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerWeaponSlot), typeof(PlayerAttack), typeof(Animator))]
[RequireComponent(typeof(PlayerLifeSystem), typeof(PlayerCollision), typeof(PlayerStats))]
[RequireComponent(typeof(PlayerPickupCollector))]

public class PlayerController : MonoBehaviour
{
    public bool CanMove => !dash.IsDashing && !attack.IsOnAttackLag && !lifeSystem.IsDead;
    public bool CanDash => dash.DashAvailable && !lifeSystem.IsDead;
    public bool CanAttack => attack.AttackAvailable && !dash.IsDashing && !lifeSystem.IsDead;
    public bool CanTakeDamage => !lifeSystem.IsInvincible && !lifeSystem.IsDead;

    PlayerInputs inputs;
    PlayerStats stats;
    PlayerMovement movement;
    PlayerDash dash;
    PlayerAttack attack;
    PlayerWeaponSlot slot;
    PlayerLifeSystem lifeSystem;
    PlayerCollision collision;
    PlayerPickupCollector collector;
    Animator animator;

    private void Awake()
    {
        GetComponentsRef();  
    }

    void GetComponentsRef()
    {
        inputs = GetComponent<PlayerInputs>();
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        dash = GetComponent<PlayerDash>();
        attack = GetComponent<PlayerAttack>();
        slot = GetComponent<PlayerWeaponSlot>();
        lifeSystem = GetComponent<PlayerLifeSystem>();
        collision = GetComponent<PlayerCollision>();
        collector = GetComponent<PlayerPickupCollector>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        InitComponentsRef();
    }

    void InitComponentsRef()
    {
        movement.InitRef(inputs, collision, stats);
        dash.InitRef(collision, lifeSystem, stats);
        attack.InitRef(slot, stats);
        lifeSystem.InitRef(animator);
        stats.InitRef(movement, slot, dash, attack);
        collector.InitRef(stats, slot, lifeSystem);
    }

    void Update()
    {
        Vector3 moveAxis = inputs.MoveAxisInput.ReadValue<Vector2>();
        Vector3 attackAxis = inputs.AttackAxisInput.ReadValue<Vector2>();

        //Rotation
        if (CanMove) 
        {
            if (moveAxis != Vector3.zero)
                movement.RotateToMoveDirection(moveAxis);

            if (attackAxis != Vector3.zero)
                movement.RotateToMoveDirection(attackAxis);

            if (inputs.AttackButtonInput.IsPressed() && inputs.IsInputScheme(inputs.AttackButtonInput, InputSchemeEnum.KeyboardMouse))
            {
                Vector3 mouseDirection = (Camera.main.ScreenToWorldPoint(inputs.MousePositionAxisInput.ReadValue<Vector2>()) - transform.position).normalized;
                movement.RotateToMoveDirection(mouseDirection);
            }

        }
        
        //Movement
        if(CanMove && moveAxis != Vector3.zero)
            movement.CheckedMove(moveAxis);

        //Attack
        if(CanAttack)
        {
            if(attack.AutoAttackOnStick && attackAxis != Vector3.zero || inputs.AttackButtonInput.IsPressed())
                attack.AttackActivation();

        }

        //Dash
        if (CanDash && inputs.DashButtonInput.WasPerformedThisFrame())
        {
            if (moveAxis != Vector3.zero)
            {
                dash.DashActivation(moveAxis);
                movement.RotateToMoveDirection(moveAxis);
            }

            else
            {
                dash.DashActivation(transform.up);
                movement.RotateToMoveDirection(transform.up);
            }

        }

        //Collision Check reaction
        if (CanTakeDamage)
        {
            if (collision.EnemyCheckCollision(collision.EnemyLayer, out int dmg))
            {
                lifeSystem.TakeDamage(dmg);
            }

            if (collision.PickupCheckCollision(collision.PickupLayer, out PickupItem item))
            {
                if (item.IsDespawning) return;

                collector.PickUpSorting(item);
            }
        }
    }

}
