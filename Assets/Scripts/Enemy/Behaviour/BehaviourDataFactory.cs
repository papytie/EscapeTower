public class BehaviourDataFactory
{
    public static IBehaviourData CreateBehaviourData(BehaviourType type)
    {
        return type switch
        {
            BehaviourType.Knight => new KnightBehaviourData(),
            BehaviourType.Eye => new EyeBehaviourData(),
            _ => null,
        };
    }

}
