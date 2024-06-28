using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HarasserData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;

    public List<ActionConfig> actions = new();

    public HarasserData()
    {
        actions.Add(new ActionConfig(ActionStateType.WaitMove));
        actions.Add(new ActionConfig(ActionStateType.RoamMove));
        actions.Add(new ActionConfig(ActionStateType.StayAtRangeMove));
        actions.Add(new ActionConfig(ActionStateType.RangedAttack));
    }
}
