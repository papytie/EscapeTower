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
    public float HitboxDuration => duration;
    public float CircleRadius => circleRadius;
    public int TargetMax => numberOfTarget;
    public int HitboxTicks => numberOfTick;
    public Vector2 BoxSize => boxSize;
    public HitboxDetectionType DetectionType => detectionType;
    public HitboxShapeType ShapeType => shapeType;
    public Vector2 HitboxCurrentPos => hitboxCurrentPos;

    [Header("Weapon Settings")]
    [SerializeField] Animator weaponAnimator;
    [SerializeField] PlayerWeaponSlot weaponSlot;
    [SerializeField] float damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;

    [Header("Hitbox Settings")]
    [SerializeField] HitboxDetectionType detectionType;
    [SerializeField] int numberOfTick = 5;
    [SerializeField] float duration = .1f;
    [SerializeField] HitboxShapeType shapeType;
    [SerializeField] float circleRadius = .1f;
    [SerializeField] Vector2 boxSize = new Vector2(.1f, .1f);
    [SerializeField] Vector2 startPos = Vector2.zero;
    [SerializeField] Vector2 targetPos = Vector2.zero;
    [SerializeField] int numberOfTarget = 1;
    [SerializeField] LayerMask enemyLayer;

    Vector2 hitboxCurrentPos = Vector2.zero;

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

    public bool WeaponHitBoxResult(out RaycastHit2D[] collisionsList, float time)
    {
        Vector2 weaponPos = transform.position;
        Vector2 weaponForward = transform.up;
        Vector2 weaponRight = transform.right;
        Vector2 hitboxStartPos = weaponPos + weaponRight * startPos.y + weaponForward * startPos.x;
        Vector2 hitboxTargetPos = weaponPos + weaponRight * targetPos.y + weaponForward * targetPos.x;
        hitboxCurrentPos = Vector2.Lerp(hitboxStartPos, hitboxTargetPos, time);

        collisionsList = shapeType switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrentPos, circleRadius, Vector2.zero, 0, enemyLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrentPos, boxSize, Quaternion.Angle(Quaternion.identity, transform.rotation), Vector2.zero, 0, enemyLayer),
            _ => null,
        };

        return collisionsList.Length > 0;
    }

    public Vector2 GetHitboxPosition(float time)
    {
        Vector2 weaponPos = transform.position;
        Vector2 weaponForward = transform.up;
        Vector2 weaponRight = transform.right;
        Vector2 hitboxStartPos = weaponPos + weaponRight * startPos.y + weaponForward * startPos.x;
        Vector2 hitboxTargetPos = weaponPos + weaponRight * targetPos.y + weaponForward * targetPos.x;
        return Vector2.Lerp(hitboxStartPos, hitboxTargetPos, time);
    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Vector2 weaponPos = transform.position;
            Vector2 weaponForward = transform.up;
            Vector2 weaponRight = transform.right;
            Vector2 hitboxStartPos = weaponPos + weaponRight * startPos.y + weaponForward * startPos.x;
            Vector2 hitboxTargetPos = weaponPos + weaponRight * targetPos.y + weaponForward * targetPos.x;

            switch (shapeType)
            {
                case HitboxShapeType.Circle:
                    Gizmos.color = startColor;
                    Gizmos.DrawWireSphere(hitboxStartPos, circleRadius);
                    Gizmos.color = Color.white;
                    Gizmos.color = endColor;
                    Gizmos.DrawWireSphere(hitboxTargetPos, circleRadius);
                    Gizmos.color = Color.white;
                    break;

                case HitboxShapeType.Box:
                    Gizmos.color = startColor;
                    Gizmos.DrawWireMesh(debugCube, -1, hitboxStartPos, transform.rotation, boxSize);
                    Gizmos.color = Color.white;
                    Gizmos.color = endColor;
                    Gizmos.DrawWireMesh(debugCube, -1, hitboxTargetPos, transform.rotation, boxSize);
                    Gizmos.color = Color.white;
                    break;
            }

        }
    }

}

public enum HitboxShapeType
{
    Box = 1,
    Circle = 2
}

public enum HitboxDetectionType
{
    EachFrame = 0,
    CustomTick = 1,
}