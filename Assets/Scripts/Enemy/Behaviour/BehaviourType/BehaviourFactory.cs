using UnityEngine;

public class BehaviourFactory
{
    public static IBehaviour Create(GameObject thisObject, BehaviourType type)
    {
        return type switch
        {
            BehaviourType.Harmless => thisObject.AddComponent<Harmless>(),
            BehaviourType.Stalker => thisObject.AddComponent<Stalker>(),
            BehaviourType.Harasser => thisObject.AddComponent<Harasser>(),
            BehaviourType.Fighter => thisObject.AddComponent<Fighter>(),
            _ => null,
        };
    }

}
