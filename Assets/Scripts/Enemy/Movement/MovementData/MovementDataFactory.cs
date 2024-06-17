public class MovementDataFactory
{
    public static IMovementData CreateData(MovementType type)
    {
        return type switch
        {
            MovementType.Wait => null,
            MovementType.Chase => new ChaseData(),
            MovementType.Flee => new FleeData(),
            MovementType.StayAtRange => new StayAtRangeData(),
            MovementType.TurnAround => new TurnAroundData(),
            _ => null,
        };

    }

}
