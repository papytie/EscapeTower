using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]

public class PlayerWeapon : MonoBehaviour
{
    public int Damage => damage;
    public float Cooldown => cooldown;
    public float Lag => lag;
    public Vector2 HitboxPos => hitboxPos;
    public Vector2 HitboxSize => hitboxSize;

    [Header("Weapon Settings")]
    [SerializeField] Animator weaponAnimator;
    [SerializeField] int damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;

    [Header("Hitbox Settings")]
    [SerializeField] float hitboxRadius = .1f;
    [SerializeField] float hitboxRange = 0;
    [SerializeField] Vector2 hitboxPos;
    [SerializeField] Vector2 hitboxSize;
    [SerializeField] float hitboxAngle = 0;
    [SerializeField] LayerMask enemyLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Color colliderDebugColor = Color.red;

    private void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    public void WeaponAttackFX()
    {
        weaponAnimator.SetTrigger(GameParams.Animation.WEAPON_ATTACK_TRIGGER);
    }

    public bool WeaponHitBoxResult(Vector2 direction, out RaycastHit2D[] collisions)
    {
        collisions = Physics2D.BoxCastAll(transform.position.ToVector2() + hitboxPos, hitboxSize, hitboxAngle, direction, hitboxRange, enemyLayer);
        return collisions.Length > 0 ? true: false; 
    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = colliderDebugColor;
            Gizmos.DrawWireCube(transform.position.ToVector2() + hitboxPos, hitboxSize);
            Gizmos.color = Color.white;

        }
    }
}
