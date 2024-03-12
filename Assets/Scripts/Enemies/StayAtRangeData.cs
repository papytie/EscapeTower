using System;

[Serializable]
public class StayAtRangeData : IMovementData
{
    public float speed = 2f;
    public float minRange = .5f;
    public float maxRange = 1f;
}
