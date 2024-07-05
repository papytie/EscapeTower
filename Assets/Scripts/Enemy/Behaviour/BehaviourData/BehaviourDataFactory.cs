public class BehaviourDataFactory
{
    public static IBehaviourData Create(BehaviourType type)
    {
        return type switch
        {
            BehaviourType.Harmless => new HarmlessData(),
            BehaviourType.Stalker => new StalkerData(),
            BehaviourType.Harasser => new HarasserData(),
            BehaviourType.Fighter => new FighterData(),
            _ => null,
        };
    }

}
