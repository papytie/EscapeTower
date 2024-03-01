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
    public bool IsSpawningProjectile => isSpawningProjectile;
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
    public HitboxRelativePosition RelativeTo => relativeTo;
    public Vector2 HitboxCurrentPos => hitboxCurrentPos;

    [Header("Weapon Settings")]
    [SerializeField] PlayerWeaponSlot weaponSlot;
    [SerializeField] float damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;
    [Header("Projectile Option")]
    [SerializeField] bool isSpawningProjectile = false;
    [SerializeField] PlayerProjectile projectileToSpawn;

    [Header("Hitbox Settings")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] HitboxRelativePosition relativeTo;
    [SerializeField] Vector2 startPositionOffset = Vector2.zero;
    [SerializeField] Vector2 targetPositionOffset = Vector2.zero;
    [SerializeField] float duration = .1f;
    [SerializeField] int numberOfTarget = 1;
    [Header("Hitbox Shape")]
    [SerializeField] HitboxShapeType shapeType;
    [SerializeField] float circleRadius = .1f;
    [SerializeField] Vector2 boxSize = new Vector2(.1f, .1f);
    [Header("Hitbox Type")]
    [SerializeField] HitboxDetectionType detectionType;
    [SerializeField] int numberOfTick = 5;

    [Header("Debug")]
    [SerializeField] bool showDebug;
    [SerializeField] Mesh debugCube;
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    Vector2 hitboxCurrentPos = Vector2.zero;
    Animator weaponAnimator;
/*
    public Vector3 HitboxStartPos => transform.position + transform.right * startPositionOffset.y + transform.up * startPositionOffset.x;
    public Vector3 HitboxTargetPos => transform.position + transform.right * targetPositionOffset.y + transform.up * targetPositionOffset.x;
    public float HitboxAngle => Quaternion.Angle(Quaternion.identity, transform.rotation);
*/
    private void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    public void InitRef(PlayerWeaponSlot slot)
    {
        weaponSlot = slot;
    }

    public void AttackFX(float playerAttackSpeed)
    {
        weaponAnimator.SetTrigger(GameParams.Animation.WEAPON_ATTACK_TRIGGER);
        weaponAnimator.SetFloat(GameParams.Animation.WEAPON_ATTACKSPEED_FLOAT, playerAttackSpeed);
    }

    public bool HitBoxResult(float time, out RaycastHit2D[] collisionsList)
    {
        GetPositions(relativeTo, out Vector2 relativeStartPos, out Vector2 relativeEndPos, out Quaternion relativeRotation);

        hitboxCurrentPos = Vector2.Lerp(relativeStartPos, relativeEndPos, time);
        collisionsList = shapeType switch
        {
            HitboxShapeType.Circle => Physics2D.CircleCastAll(hitboxCurrentPos, circleRadius, Vector2.zero, 0, enemyLayer),
            HitboxShapeType.Box => Physics2D.BoxCastAll(hitboxCurrentPos, boxSize, Quaternion.Angle(Quaternion.identity, relativeRotation), Vector2.zero, 0, enemyLayer),
            _ => null,
        };
        return collisionsList.Length > 0;
    }

    public void GetPositions(HitboxRelativePosition relativeTo, out Vector2 relativeStartPos, out Vector2 relativeEndPos, out Quaternion relativeRotation)
    {
        switch (relativeTo)
        {
            case HitboxRelativePosition.World:
                relativeStartPos = startPositionOffset;
                relativeEndPos = targetPositionOffset;
                relativeRotation = Quaternion.identity;
                break;

            case HitboxRelativePosition.Weapon:
                relativeStartPos = transform.position + transform.right * startPositionOffset.y + transform.up * startPositionOffset.x;
                relativeEndPos = transform.position + transform.right * targetPositionOffset.y + transform.up * targetPositionOffset.x;
                relativeRotation = transform.rotation;
                break;

            case HitboxRelativePosition.Player:
                relativeStartPos = weaponSlot.transform.position + weaponSlot.transform.right * startPositionOffset.y + weaponSlot.transform.up * startPositionOffset.x;
                relativeEndPos = weaponSlot.transform.position + weaponSlot.transform.right * targetPositionOffset.y + weaponSlot.transform.up * targetPositionOffset.x;
                relativeRotation = weaponSlot.transform.rotation;
                break;

            case HitboxRelativePosition.WeaponSlot:
                relativeStartPos = weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.right * startPositionOffset.y + weaponSlot.SlotTransform.up * startPositionOffset.x;
                relativeEndPos = weaponSlot.SlotTransform.position + weaponSlot.SlotTransform.right * targetPositionOffset.y + weaponSlot.SlotTransform.up * targetPositionOffset.x;
                relativeRotation = weaponSlot.SlotTransform.rotation;
                break;

            default:
                relativeStartPos = Vector2.zero;
                relativeEndPos = Vector2.zero;
                relativeRotation = Quaternion.identity;
                break;
        }

    }

    public void SpawnProjectile()
    {
        GetPositions(relativeTo, out Vector2 startPos, out Vector2 targetPos, out Quaternion rotation);
        Instantiate(projectileToSpawn, startPos, rotation).Init(this, targetPos);
    }


    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            GetPositions(relativeTo, out Vector2 startPos, out Vector2 targetPos, out Quaternion rotation);
            switch (shapeType)
            {
                case HitboxShapeType.Circle:
                    Gizmos.color = startColor;
                    Gizmos.DrawWireSphere(startPos, circleRadius);
                    Gizmos.color = Color.white;
                    Gizmos.color = endColor;
                    Gizmos.DrawWireSphere(targetPos, circleRadius);
                    Gizmos.color = Color.white;
                    break;

                case HitboxShapeType.Box:
                    Gizmos.color = startColor;
                    Gizmos.DrawWireMesh(debugCube, -1, startPos, rotation, boxSize);
                    Gizmos.color = Color.white;
                    Gizmos.color = endColor;
                    Gizmos.DrawWireMesh(debugCube, -1, targetPos, rotation, boxSize);
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

public enum HitboxRelativePosition
{
    World = 0,
    Weapon = 1,
    Player = 2,
    WeaponSlot = 3,
}