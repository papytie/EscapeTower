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
    public float HitboxRadius => hitboxRadius;
    public float HitboxRange => hitboxRange;
    public float HitboxDuration => hitboxDuration;

    [Header("Weapon Settings")]
    [SerializeField] Animator weaponAnimator;
    [SerializeField] int damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;

    [Header("Hitbox Settings")]
    [SerializeField] float hitboxRadius = .1f;
    [SerializeField] float hitboxRange = .1f;
    [SerializeField] float hitboxDuration = .1f;
    [SerializeField] LayerMask enemyLayer;

    private void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    public void WeaponAttackFX()
    {
        weaponAnimator.SetTrigger(GameParams.Animation.WEAPON_ATTACK_TRIGGER);
    }

    public bool WeaponHitBoxResult(Vector2 origin, Vector2 direction, out RaycastHit2D[] collisions)
    {
        collisions = Physics2D.CircleCastAll(origin, hitboxRadius, direction, hitboxRange, enemyLayer);

        return collisions.Length > 0 ? true: false; 
    }

}
