using System;
using UnityEngine;

[Serializable]
public class TakeDamageData : IActionData
{
    public float recupTime = 1;
    [Header("Bump Settings")]
    public bool activation = false;
    public AnimationCurve moveCurve;
    [Header("Bump")]
    public DropItemConfig dropConfig;
}
