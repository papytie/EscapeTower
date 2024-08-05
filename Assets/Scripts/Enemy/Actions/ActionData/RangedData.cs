using System;
using UnityEngine;

[Serializable]
public class RangedData : IActionData
{
    public ProjectileData projectileData;

    public float baseDamage = 1;
    public float activationRange = 2;
    public float reactionTime = 1;
    public float duration = 1;
    public float recupTime = 1;
    public float cooldown = 1;

    public bool displayWarningBeam = false;
    public float warningTime = 1;
    public AnimatedBeam warningBeam;

    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color debugColor = new(1,0,0,.5f);
}
