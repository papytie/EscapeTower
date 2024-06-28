//All Enums for enemy scripts

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

public enum Condition
{
    HaveTarget = 0,
    CurrentHealthAboveHalf = 1,
    CurrentHealthBelowHalf = 2,
    CurrentHealthFull = 3,
    InAttackRange = 4,
    OutOfAttackRange = 5,
}

public enum ActionStateType
{
    WaitMove = 0,
    RoamMove = 1,
    ChaseMove = 2,
    StayAtRangeMove = 3,
    FleeMove = 4,
    TurnAroundMove = 5,
    MeleeAttack = 6,
    RangedAttack = 7,
    ChargeAttack = 8,
}

public enum BehaviourType
{
    Harmless = 0,
    Stalker = 1,
    Harasser = 2,
}