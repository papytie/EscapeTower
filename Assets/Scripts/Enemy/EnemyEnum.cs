//All Enums for enemy scripts

public enum MovementType
{
    Wait = 0,
    Chase = 1,
    Flee = 2,
    StayAtRange = 3,
    TurnAround = 4,
}

public enum AttackType
{
    Melee = 0,
    Ranged = 1,
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

public enum Condition
{
    HaveTarget = 0,
    CurrentHealthAboveHalf = 1,
    CurrentHealthBelowHalf = 2,
    CurrentHealthFull = 3,
    InAttackRange = 4,
    OutOfAttackRange = 5,
}

public enum StateType
{
    Wait = 0,
    SearchMove = 1,
    ChaseMove = 2,
    FleeMove = 3,
    StayAtRangeMove = 4,
    MeleeAttack = 5,
    RangedAttack = 6,
}