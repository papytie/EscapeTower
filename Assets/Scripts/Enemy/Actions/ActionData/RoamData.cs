using System;

[Serializable]
public class RoamData : IActionData
{
    public float speedMult = .5f;
    public float minTime = 1;
    public float maxTime = 5;
    public int maxBounce = 3;
}
