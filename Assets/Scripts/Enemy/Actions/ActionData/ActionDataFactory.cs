public class ActionDataFactory
{
    public static IActionData CreateData(ActionType type)
    {
        return type switch
        {
            ActionType.WaitMove => new WaitData(),
            ActionType.ChaseMove => new ChaseData(),
            ActionType.FleeMove => new FleeData(),
            ActionType.StayAtRangeMove => new StayAtRangeData(),
            ActionType.TurnAroundMove => new TurnAroundData(),
            ActionType.RoamMove => new RoamData(),
            ActionType.MeleeAttack => new MeleeData(),
            ActionType.RangedAttack => new RangedData(),
            ActionType.ChargeAttack => new ChargeData(),
            ActionType.TakeDamageReaction => new TakeDamageData(),
            ActionType.DieReaction => new DieData(),
            ActionType.BeamAttack => new BeamData(),
            ActionType.EmptyAction => new EmptyData(),
            ActionType.DropAction => new DropData(),
            _ => null,
        };
    }
}
