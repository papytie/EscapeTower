using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "GameData/Enemy/AttackData", order = 1)]

[Serializable]
public class AttackData : ScriptableObject
{
    [Header("Attack Settings")]
    public float damage = 1;
    public float cooldown = .5f;
    public float lag = .2f;
    public float delay = 0;
    public float duration = .1f;
    public LayerMask playerLayer;

    [Header("Melee Settings")]
    public bool useMeleeHitbox = true;
    public float hitboxDuration = .1f;
    public int maxTargets = 1;
    public Vector2 hitboxPositionOffset = Vector2.zero;
    public HitboxShapeType hitboxShape;
    public float circleRadius = .1f;
    public Vector2 boxSize = new(.1f, .1f);
    public HitboxBehaviorType behaviorType;
    public AnimationCurve hitboxMovementCurve;
    public Vector2 targetPosition = Vector2.zero;

    [Header("Projectile Option")]
    public bool useProjectile = false;
    public AnimationCurve launchCurve;
    public ProjectileReturnType projectileReturnType;
    public bool projectileReturnFlip = false;
    public AnimationCurve returnCurve;
    public PlayerProjectile projectileToSpawn;
    public RelativeTransform projectileSpawnRelativeTo;
    public Vector2 projectileSpawnOffset = Vector2.zero;
    public float projectileAngleOffset = 0;
    public int projectileNumber = 1;
    public ProjectileSpawnType projectileSpawnType;
    public int projectileMaxTargets = 1;
    public float projectileSpeed = 1;
    public float spreadAngle = 60;
    public float projectileRange = 1;
}
