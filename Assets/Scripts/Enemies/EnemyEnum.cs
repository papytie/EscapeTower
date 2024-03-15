//All Enums for enemy scripts

public enum MovementType
{
    Wait = 0,
    Chase = 1,
    Flee = 2,
    StayAtRange = 3,
    TurnAround = 4,
}

public enum TurnDirection
{
    Clockwise = 0,
    Anticlockwise = 1,
}

public enum LookDirection
{
    Movement = 0,
    Target = 1,
    TargetInvert = 2,
    MovementInvert = 3,
}
