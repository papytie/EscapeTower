using System;
using UnityEngine;

[Serializable]
public class TakeDamageData : IActionData
{
    public float recupTime = 1;
    [Header("Bump")]
    public bool activation = false;
    public AnimationCurve moveCurve;
}
