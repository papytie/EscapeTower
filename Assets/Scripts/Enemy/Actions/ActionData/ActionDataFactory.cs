public class ActionDataFactory
{
    public static IActionData CreateData(ActionStateType type)
    {
        return type switch
        {
            ActionStateType.WaitMove => new WaitData(),
            ActionStateType.ChaseMove => new ChaseData(),
            ActionStateType.FleeMove => new FleeData(),
            ActionStateType.StayAtRangeMove => new StayAtRangeData(),
            ActionStateType.TurnAroundMove => new TurnAroundData(),
            ActionStateType.RoamMove => new RoamData(),
            ActionStateType.MeleeAttack => new MeleeData(),
            ActionStateType.RangedAttack => new RangedData(),
            ActionStateType.ChargeAttack => new ChargeData(),
            _ => null,
        };
    }
}
