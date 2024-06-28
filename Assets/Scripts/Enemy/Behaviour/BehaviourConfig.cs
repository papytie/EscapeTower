using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourConfig", menuName = "GameData/Behaviour", order = 1)]

public class BehaviourConfig : ScriptableObject
{
    public BehaviourType behaviourType;
    [SerializeReference] public IBehaviourData Data;
    public IBehaviour behaviour;

    private void Awake()
    {
        InitData();
    }

    void InitData()
    {
        Data = BehaviourDataFactory.CreateBehaviourData(behaviourType);

        foreach (ActionConfig action in Data.Actions)
            action.data = ActionDataFactory.CreateData(action.StateType);
    }

    private void OnValidate()
    {
        Type type = behaviourType switch
        {
            BehaviourType.Harmless => typeof(HarmlessData),
            BehaviourType.Stalker => typeof(StalkerData),
            BehaviourType.Harasser => typeof(HarasserData),
            _ => null,
        };

        if ((Data == null && type != null) || (Data != null && Data.GetType() != type))
            InitData();
    }
}
