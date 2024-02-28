using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]

public class PlayerWeapon : MonoBehaviour
{
    public float Damage => damage;
    public float Cooldown => cooldown;
    public float Lag => lag;
    public float HitboxDuration => hitboxDuration;
    public int TargetMax => targetMax;

    [Header("Weapon Settings")]
    [SerializeField] Animator weaponAnimator;
    [SerializeField] PlayerWeaponSlot weaponSlot;
    [SerializeField] float damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;
    [SerializeField] float hitboxDuration = .1f;

    [Header("Hitbox Settings")]
    [SerializeField] HitboxShape hitboxShape;
    [SerializeField] float hitboxCircleRadius = .1f;
    [SerializeField] Vector2 hitboxBoxSize = Vector2.zero;
    [SerializeField] float hitboxMinDist = 0;
    [SerializeField] float hitboxMaxDist = 0;
    [SerializeField] int targetMax;
    [SerializeField] LayerMask enemyLayer;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;


    private void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    public void InitRef(PlayerWeaponSlot slot)
    {
        weaponSlot = slot;
    }

    public void WeaponAttackFX(float playerAttackSpeed)
    {
        weaponAnimator.SetTrigger(GameParams.Animation.WEAPON_ATTACK_TRIGGER);
        weaponAnimator.SetFloat(GameParams.Animation.WEAPON_ATTACKSPEED_FLOAT, playerAttackSpeed);
    }

    public bool WeaponHitBoxResult(out RaycastHit2D[] collisionsList)
    {
        collisionsList = hitboxShape switch
        {
            HitboxShape.Circle => Physics2D.CircleCastAll(weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.up * hitboxMinDist, hitboxCircleRadius, weaponSlot.SlotTransform.up, hitboxMaxDist, enemyLayer),
            HitboxShape.Box => Physics2D.BoxCastAll(weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.up * hitboxMinDist, hitboxBoxSize, Quaternion.Angle(Quaternion.identity, weaponSlot.transform.rotation), weaponSlot.SlotTransform.up, hitboxMaxDist, enemyLayer),
            _ => null,
        };
        return collisionsList.Length > 0;
    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            switch (hitboxShape)
            {
                case HitboxShape.Circle:
                    Gizmos.color = startColor;
                    Gizmos.DrawSphere(weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.up * hitboxMinDist, hitboxCircleRadius);
                    Gizmos.color = endColor;
                    Gizmos.DrawSphere(weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.up * hitboxMaxDist, hitboxCircleRadius);
                    break;

                case HitboxShape.Box:
                    Gizmos.color = startColor;
                    Gizmos.DrawMesh(debugCube, -1, weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.up * hitboxMinDist, weaponSlot.SlotTransform.rotation, hitboxBoxSize);
                    Gizmos.color = endColor;
                    Gizmos.DrawMesh(debugCube, -1, weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.up * hitboxMaxDist, weaponSlot.SlotTransform.rotation, hitboxBoxSize);
                    break;
            }

        }
    }

}

enum HitboxShape
{
    Box = 1,
    Circle = 2
}
