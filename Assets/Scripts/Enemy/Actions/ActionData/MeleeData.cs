using System;
using UnityEngine;

[Serializable]
public class MeleeData : IActionData
{
    [Header("HitboxSettings")]
    public HitboxSettingsData hitbox;
    public MeleeAttackEffect attackEffect;

    [Header("AttackSettings")]
    public float baseDamage = 1;
    public float activationRange = .5f;
    public float reactionTime = 1;
    public float recupTime = 1;
    public float cooldown = 1;

    [Header("Debug")]
    public bool showDebug = true;
    public Mesh debugCube;
    public Color debugColor = Color.white;
}
