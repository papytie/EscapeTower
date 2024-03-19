using System;

[Serializable]
public class TurnAroundData : IMovementData
{
    public float speed = 2f;
    public TurnDirection direction;
    public LookDirection look;
}
