using System;

[Serializable]
public class TurnAroundData : IMovementData
{
    public float speedMult = 2f;
    public TurnDirection direction;
    public LookDirection look;
}
