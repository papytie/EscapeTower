using System;
using UnityEngine;

[Serializable]
public class MeleeData : IActionData
{
    public HitboxSettingsData hitbox;

    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color debugColor = Color.white;
}
