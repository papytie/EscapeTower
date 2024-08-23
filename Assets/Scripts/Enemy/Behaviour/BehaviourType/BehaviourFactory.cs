using UnityEngine;

public class BehaviourFactory
{
    public static IBehaviour Create(GameObject thisObject, BehaviourType type)
    {
        return type switch
        {
            BehaviourType.Harmless => thisObject.AddComponent<Harmless>(),
            BehaviourType.Stalker => thisObject.AddComponent<Stalker>(),
            BehaviourType.Hunter => thisObject.AddComponent<Hunter>(),
            BehaviourType.Fighter => thisObject.AddComponent<Fighter>(),
            BehaviourType.MultiAttackBoss => thisObject.AddComponent<MultiAttackBoss>(),
            _ => null,
        };
    }

}
