using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputs), typeof(PlayerMovement), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerWeaponSlot), typeof(PlayerAttack), typeof(Animator))]
[RequireComponent(typeof(PlayerLifeSystem))]
public class PlayerController : MonoBehaviour
{
    public bool CanMove => !dash.IsDashing && !attack.IsAttacking && !lifeSystem.IsDead;
    public bool CanDash => dash.DashAvailable && !lifeSystem.IsDead;
    public bool CanAttack => attack.AttackAvailable && !dash.IsDashing && !lifeSystem.IsDead;

    PlayerInputs inputs;
    PlayerMovement movement;
    PlayerDash dash;
    PlayerAttack attack;
    PlayerWeaponSlot slot;
    PlayerLifeSystem lifeSystem;
    Animator animator;

    private void Awake()
    {
        GetComponentsRef();  
    }

    void Start()
    {
        InitComponentsRef();
    }

    void Update()
    {
        if (CanMove)
            movement.Move();
        
        if (inputs.DashButtonInput.WasPerformedThisFrame() && CanDash)
        {
            Vector3 moveAxis = inputs.MoveAxisInput.ReadValue<Vector2>();

            if (moveAxis != Vector3.zero)
                dash.DashActivation(moveAxis);

            else dash.DashActivation(transform.up);
        }

        if (inputs.AttackButtonInput.WasPerformedThisFrame() && CanAttack)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(inputs.MousePositionAxisInput.ReadValue<Vector2>());
            attack.AttackActivation((mouseWorldPos - transform.position).normalized);

        }

        Vector3 attackAxis = inputs.AttackAxisInput.ReadValue<Vector2>();
        if (attackAxis != Vector3.zero && CanAttack)
            attack.AttackActivation(attackAxis);

    }

    void GetComponentsRef()
    {
        inputs = GetComponent<PlayerInputs>();
        movement = GetComponent<PlayerMovement>();
        dash = GetComponent<PlayerDash>();
        attack = GetComponent<PlayerAttack>();
        slot = GetComponent<PlayerWeaponSlot>();
        lifeSystem = GetComponent<PlayerLifeSystem>();
        animator = GetComponent<Animator>();
    }

    void InitComponentsRef()
    {
        movement.InitRef(inputs, animator);
        dash.InitRef(inputs);
        attack.InitRef(inputs, slot);
        lifeSystem.InitRef(animator);
    }

}
