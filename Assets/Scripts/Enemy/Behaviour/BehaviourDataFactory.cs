public class BehaviourDataFactory
{
    public static IBehaviourData CreateBehaviourData(BehaviourType type)
    {
        return type switch
        {
            BehaviourType.Harmless => new HarmlessData(),
            BehaviourType.Stalker => new StalkerData(),
            BehaviourType.Harasser => new HarasserData(),
            _ => null,
        };
    }

}
