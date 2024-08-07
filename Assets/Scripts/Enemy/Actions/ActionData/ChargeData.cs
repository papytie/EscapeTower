using System;
using UnityEngine;

[Serializable]
public class ChargeData : IActionData
{
    public AnimationCurve moveCurve;
    public LayerMask obstructionLayer;

    public float activationRange = .5f;
    public float effectiveRange = .5f;
    public float reactionTime = 1;
    public float recupTime = 1;
    public float cooldown = 1;

    [Header("Debug")]
    public bool showDebug = false;
    public Color rangeColor = Color.yellow;
    public Color startColor = Color.green;
    public Color targetColor = Color.red;
}
