using System;
using UnityEngine;

[Serializable]
public class HitboxSettingsData
{
    [Header("Hitbox settings")]
    public HitboxShapeType shape = HitboxShapeType.Circle;
    public Vector2 boxSize = new(.1f, .1f);
    public float circleRadius = .1f;
    public float duration = .5f;
    public float delay = .5f;
    public int maxTargets = 1;
    public LayerMask detectionLayer;
    public Vector2 startPosOffset = Vector2.zero;
    public HitboxBehaviorType behaviorType = HitboxBehaviorType.Fixed;
    public Vector2 endPos = Vector2.zero;
    public AnimationCurve moveCurve;

}
