using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourConfig", menuName = "GameData/Behaviour", order = 1)]

public class BehaviourConfig : ScriptableObject
{
    public BehaviourType behaviourType;
    [SerializeReference] public IBehaviourData data;
    public IBehaviour behaviour;

    private void OnValidate()
    {
        Type type = behaviourType switch
        {
            BehaviourType.Harmless => typeof(HarmlessData),
            BehaviourType.Stalker => typeof(StalkerData),
            BehaviourType.Harasser => typeof(HarasserData),
            BehaviourType.Fighter => typeof(FighterData),
            BehaviourType.BulletHell => typeof(BulletHellData),
            _ => null,
        };

        if ((data == null && type != null) || (data != null && data.GetType() != type))
        {
            data = BehaviourDataFactory.Create(behaviourType);

            foreach (ActionConfig actionConfig in data.Actions)
                actionConfig.data = ActionDataFactory.CreateData(actionConfig.ActionType);
        }
    }
}
