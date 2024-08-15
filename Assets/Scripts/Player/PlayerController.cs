using UnityEngine;

[RequireComponent(typeof(PlayerInputs), typeof(PlayerMovement), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerWeaponSlot), typeof(PlayerPickupCollector), typeof(Animator))]
[RequireComponent(typeof(PlayerLifeSystem), typeof(CollisionCheckerComponent), typeof(PlayerStats))]
[RequireComponent(typeof(BumpComponent))]

public class PlayerController : MonoBehaviour
{
    public bool HaveWeapon => weaponSlot.EquippedWeapon;
    public bool CanMove => bump.CanMove && !dash.IsDashing /*&& !weaponSlot.EquippedWeapon.IsOnAttackLag*/ && !lifeSystem.IsDead;
    public bool CanDash => bump.CanMove && dash.DashAvailable && !lifeSystem.IsDead;
    public bool CanAttack => bump.CanMove && weaponSlot.EquippedWeapon && weaponSlot.EquippedWeapon.AttackAvailable && !dash.IsDashing && !lifeSystem.IsDead;
    public bool CanTakeDamage => !lifeSystem.IsInvincible && !lifeSystem.IsDead;
    public Vector2 MoveInput => moveInput;
    public PlayerMovement Movement => movement;

    [SerializeField] bool stickAutoAttack;

    PlayerInputs inputs;
    PlayerStats stats;
    PlayerMovement movement;
    PlayerDash dash;
    PlayerWeaponSlot weaponSlot;
    PlayerLifeSystem lifeSystem;
    CollisionCheckerComponent collision;
    PlayerPickupCollector collector;
    Animator animator;
    BumpComponent bump;

    Vector2 lastInputDirection = Vector2.zero;
    Vector2 moveInput = Vector2.zero;

    private void Awake()
    {
        GetComponentsRef();  
        InitComponentsRef();
    }

    void GetComponentsRef()
    {
        inputs = GetComponent<PlayerInputs>();
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        dash = GetComponent<PlayerDash>();
        weaponSlot = GetComponent<PlayerWeaponSlot>();
        lifeSystem = GetComponent<PlayerLifeSystem>();
        collision = GetComponent<CollisionCheckerComponent>();
        collector = GetComponent<PlayerPickupCollector>();
        animator = GetComponent<Animator>();
        bump = GetComponent<BumpComponent>();
    }

    void InitComponentsRef()
    {
        movement.InitRef(inputs, collision, stats);
        dash.InitRef(collision, lifeSystem, stats);
        lifeSystem.InitRef(animator, bump);
        stats.InitRef(movement, weaponSlot, dash);
        weaponSlot.InitRef(stats);
        collector.InitRef(stats, weaponSlot, lifeSystem);
        bump.InitRef(collision);
        collision.Init();
    }

    void Update()
    {
        moveInput = inputs.MoveAxisInput.ReadValue<Vector2>();
        Vector3 attackAxis = inputs.AttackAxisInput.ReadValue<Vector2>();
        animator.SetFloat(SRAnimators.PlayerAnimator.Parameters.moveUp, moveInput.y);
        animator.SetFloat(SRAnimators.PlayerAnimator.Parameters.moveRight, moveInput.x);

        //Movement
        if (CanMove)
        {
            if (moveInput != Vector2.zero)
            {
                if (HaveWeapon && weaponSlot.EquippedWeapon.IsOnAttackLag) return;
                movement.CheckedMove(moveInput);
                lastInputDirection = moveInput;
            }

            if (inputs.AttackButtonInput.IsPressed() && inputs.IsInputScheme(inputs.AttackButtonInput, InputSchemeEnum.KeyboardMouse))
            {
                Vector3 mouseDirection = (Camera.main.ScreenToWorldPoint(inputs.MousePositionAxisInput.ReadValue<Vector2>()) - transform.position).normalized;
                weaponSlot.RotateSlot(mouseDirection);
                lastInputDirection = mouseDirection;
                //lastAttackInputDirection = mouseDirection;
            }

            if (attackAxis != Vector3.zero)
            {
                weaponSlot.RotateSlot(attackAxis);
                lastInputDirection = attackAxis;
                //lastAttackInputDirection = attackAxis;
            }

            if (attackAxis == Vector3.zero && !inputs.AttackButtonInput.IsPressed())
                weaponSlot.RotateSlot(lastInputDirection);
        }


        //Attack
        if(CanAttack)
        {
            if(stickAutoAttack && attackAxis != Vector3.zero || inputs.AttackButtonInput.IsPressed())
            {
                animator.SetFloat(SRAnimators.PlayerAnimator.Parameters.attackSpeed, stats.GetModifiedMainStat(MainStat.AttackSpeed));
                animator.SetTrigger(SRAnimators.PlayerAnimator.Parameters.attack);
                animator.SetFloat(SRAnimators.PlayerAnimator.Parameters.attackUp, lastInputDirection.y);
                animator.SetFloat(SRAnimators.PlayerAnimator.Parameters.attackRight, lastInputDirection.x);
                weaponSlot.EquippedWeapon.AttackActivation();
            }
        }

        //Dash
        if (CanDash && inputs.DashButtonInput.WasPerformedThisFrame())
        {
            if (moveInput != Vector2.zero)
                dash.DashActivation(moveInput);
            else 
                dash.DashActivation(lastInputDirection);
        }
        if (dash.IsDashing)
            animator.SetBool(SRAnimators.PlayerAnimator.Parameters.isDashing, true);
        else animator.SetBool(SRAnimators.PlayerAnimator.Parameters.isDashing, false);

        //Collision Checks & Reactions
        if (collision.ObjectTriggerCheck(collision.IntractionObjectsLayer, out RaycastHit2D anyHit))
        {
            if (CanTakeDamage && anyHit.transform.TryGetComponent(out EnemyStatsComponent enemyStats))
            {
                lifeSystem.TakeDamage(enemyStats.CollisionDamage, -anyHit.normal);
            }

            else if (anyHit.transform.TryGetComponent(out PickupItem item)) 
            {
                if (item.IsDespawning) return;
                else collector.PickUpSorting(item);
            }
        }
    }

}
