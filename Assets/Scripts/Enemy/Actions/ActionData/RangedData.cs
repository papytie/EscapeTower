using System;
using UnityEngine;

[Serializable]
public class RangedData : IActionData
{
    public ProjectileData projectileData;

    public float baseDamage = 1;
    public float activationRange = 1;
    public float reactionTime = 1;
    public float duration = 1;
    public float recupTime = 1;
    public float cooldown = 1;

    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color debugColor = new(1,0,0,.5f);
}
