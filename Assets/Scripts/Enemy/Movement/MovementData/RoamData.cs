using System;

[Serializable]
public class RoamData : IMovementData
{
    public float speedMult = .5f;
    public float minTime = 1;
    public float maxTime = 3;
}
