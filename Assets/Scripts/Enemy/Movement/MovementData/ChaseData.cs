using System;

[Serializable]
public class ChaseData : IMovementData
{
    public float speedMult = 1;
    public float minRange = .1f;
}
