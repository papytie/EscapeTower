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
            BehaviourType.BulletHell => new BulletHellData(),
            BehaviourType.AdditiveLayer => new AdditiveLayerData(),
            _ => null,
        };
    }

}
