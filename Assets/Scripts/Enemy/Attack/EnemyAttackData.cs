using System;
using UnityEngine;

[Serializable]
public class EnemyAttackData
{
    [Header("Attack Settings")]
    public float damage = 1;
    public float cooldown = .5f;
    public float lag = .2f;
    public float delay = 0;
    public float duration = .1f;
    public LayerMask targetLayer;

    [Header("Melee Settings")]
    public bool useMeleeHitbox = true;
    public float hitboxDuration = .1f;
    public int maxTargets = 1;
    public Vector2 hitboxPositionOffset = Vector2.zero;
    public HitboxShapeType hitboxShape = HitboxShapeType.Circle;
    public float circleRadius = .1f;
    public Vector2 boxSize = new(.1f, .1f);
    public HitboxBehaviorType behaviorType = HitboxBehaviorType.Fixed;
    public AnimationCurve hitboxMovementCurve;
    public Vector2 targetPosition = Vector2.zero;

    [Header("Projectile Data")]
    public ProjectileData projectileData;
}
