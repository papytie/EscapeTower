using System;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionConfig", menuName = "GameData/Action", order = 1)]

public class ActionConfig : ScriptableObject
{
    public ActionType actionType;
    [SerializeReference] public IActionData data;

    private void OnValidate()
    {
        Type type = actionType switch
        {
            ActionType.WaitMove => typeof(WaitData),
            ActionType.RoamMove => typeof(RoamData),
            ActionType.ChaseMove => typeof(ChaseData),
            ActionType.StayAtRangeMove => typeof(StayAtRangeData),
            ActionType.FleeMove => typeof(FleeData),
            ActionType.TurnAroundMove => typeof(TurnAroundData),
            ActionType.MeleeAttack => typeof(MeleeData),
            ActionType.RangedAttack => typeof(RangedData),
            ActionType.ChargeAttack => typeof(ChargeData),
            ActionType.TakeDamageReaction => typeof(TakeDamageData),
            ActionType.DieReaction => typeof(DieData),
            ActionType.BeamAttack => typeof(BeamData),
            ActionType.EmptyAction => typeof(EmptyData),
            _ => null,
        };

        if ((data == null && type != null) || (data != null && data.GetType() != type))
        {
            data = ActionDataFactory.CreateData(actionType);
        }
    }

}