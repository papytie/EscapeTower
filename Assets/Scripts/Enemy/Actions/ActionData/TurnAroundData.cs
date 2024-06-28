using System;

[Serializable]
public class TurnAroundData : IActionData
{
    public float speedMult = 2f;
    public TurnDirection direction;
    public LookDirection look;
}
