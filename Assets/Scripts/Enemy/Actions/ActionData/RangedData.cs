using System;
using UnityEngine;

[Serializable]
public class RangedData : IActionData
{
    public ProjectileData projectileData;
    public float duration = .5f;
    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color debugColor = new(1,0,0,.5f);
}
