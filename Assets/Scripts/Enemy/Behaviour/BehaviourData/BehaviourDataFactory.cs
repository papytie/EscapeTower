public class BehaviourDataFactory
{
    public static IBehaviourData Create(BehaviourType type)
    {
        return type switch
        {
            BehaviourType.Harmless => new HarmlessData(),
            BehaviourType.Stalker => new StalkerData(),
            BehaviourType.Hunter => new HunterData(),
            BehaviourType.Fighter => new FighterData(),
            BehaviourType.Hybrid => new HybridData(),
            BehaviourType.MultiAttackBoss => new MultiAttackBossData(new AdditiveBehaviourData()),
            _ => null,
        };
    }

}
