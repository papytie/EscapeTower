using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourConfig", menuName = "GameData/Behaviour", order = 1)]

public class BehaviourConfig : ScriptableObject
{
    public BehaviourType behaviourType;
    [SerializeReference] public IBehaviourData data;

    private void OnValidate()
    {
        Type type = behaviourType switch
        {
            BehaviourType.Harmless => typeof(HarmlessData),
            BehaviourType.Stalker => typeof(StalkerData),
            BehaviourType.Hunter => typeof(HunterData),
            BehaviourType.Fighter => typeof(FighterData),
            BehaviourType.MultiAttackBoss => typeof(MultiAttackBossData),
            _ => null,
        };

        if ((data == null && type != null) || (data != null && data.GetType() != type))
        {
            data = BehaviourDataFactory.Create(behaviourType);
            //additiveBehaviourData = new AdditiveBehaviourData();
        }
    }
}
