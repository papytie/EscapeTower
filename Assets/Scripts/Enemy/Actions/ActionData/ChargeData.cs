using System;
using UnityEngine;

[Serializable]
public class ChargeData : IActionData
{
    [Header("Movement Settings")]
    public AnimationCurve moveCurve;
    public LayerMask obstructionLayer;
    public float activationRange = .5f;
    public float effectiveRange = .5f;
    public float reactionTime = 1;
    public float recupTime = 1;
    public float cooldown = 1;

    [Header("Drop Item")]
    public DropItemConfig dropConfig;

    [Header("Debug")]
    public bool showDebug = false;
    public Color rangeColor = Color.yellow;
    public Color startColor = Color.green;
    public Color targetColor = Color.red;
}
