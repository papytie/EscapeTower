using System;
using UnityEngine;

[Serializable]
public class MeleeData : IActionData
{
    public int priority = 0;
    public float cooldown = .5f;
    public float lag = .2f;
    public float delay = 0;
    public float duration = .1f;
    public HitboxSettingsData hitboxSettings;

    [Header("Move Settings")]
    public float speedMult = .5f;
    public float minRange = .1f;
    [Header("Duration")]
    public float processMinTime = 1;
    public float processMaxTime = 1;
    [Header("Cooldown")]
    public float cooldownMinTime = 0;
    public float cooldownMaxTime = 0;

}
