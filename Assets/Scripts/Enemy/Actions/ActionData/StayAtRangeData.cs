using System;

[Serializable]
public class StayAtRangeData : IActionData
{
    public float speedMult = 1f;
    public float minRange = 1f;
    public float maxRange = 2f;
}
