using System;
using UnityEngine;

[Serializable]
public class BeamData : IActionData
{
    public AnimatedBeam beamVisual;

    public LayerMask obstructionLayer;
    public LayerMask targetLayer;
    public Vector2 spawnOffset = Vector2.zero;
    public int spawnNumber = 1;
    public float spreadAngle = 60;

    public float baseDamage = 1;
    public float activationRange = 1;
    public float effectiveRange = 2;
    public float aimingDuration = 1;
    public float fireDuration = 1;
    public float recupTime = 1;
    public float cooldown = 1;

    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color debugColor = new(1,0,0,.5f);
}