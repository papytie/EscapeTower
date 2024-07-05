using System;
using UnityEngine;

[Serializable]
public class TakeDamageData : IActionData
{
    [Header("Bump")]
    public bool activation = false;
    public AnimationCurve moveCurve;
}
