using System;
using UnityEngine;

[Serializable]
public class RangedData : IActionData
{
    public ProjectileData projectileData;

    public float baseDamage = 1;
    public float activationRange = 0;
    public float reactionTime = .2f;
    public float duration = .1f;
    public float recupTime = .5f;
    public float cooldown = 1;

    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color debugColor = new(1,0,0,.5f);
}
