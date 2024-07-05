using System;

[Serializable]
public class TurnAroundData : IActionData
{
    public float speedMult = 1f;
    public TurnDirection direction;
    public LookDirection look;
}
