using System;
using UnityEngine;

[Serializable]
public class HitboxSettingsData
{
    [Header("Hitbox shape")]
    public HitboxShapeType hitboxShape = HitboxShapeType.Circle;
    public Vector2 boxSize = new(.1f, .1f);
    public float circleRadius = .1f;

    [Header("Hitbox settings")]
    public LayerMask activeLayer;
    public float hitboxDuration = .1f;
    public int maxTargets = 1;
    public Vector2 hitboxStartPosOffset = Vector2.zero;
    public HitboxBehaviorType behaviorType = HitboxBehaviorType.Fixed;
    public Vector2 hitboxEndPos = Vector2.zero;
    public AnimationCurve hitboxMovementCurve;

    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color meleeDebugColor;

}
