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

public enum ActionType
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
    TakeDamageReaction = 9,
    DieReaction = 10,
    BeamAttack = 11,
    EmptyAction = 12,
}

public enum BehaviourType
{
    Harmless = 0,
    Stalker = 1,
    Hunter = 2,
    Fighter = 3,
    MultiAttackBoss = 4,
    Hybrid = 5,
}