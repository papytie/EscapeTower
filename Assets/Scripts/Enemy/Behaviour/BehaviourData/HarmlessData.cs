using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HarmlessData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;

    public List<ActionConfig> actions = new();

    public HarmlessData()
    {
        actions.Add(new ActionConfig(ActionStateType.WaitMove));
        actions.Add(new ActionConfig(ActionStateType.RoamMove));
        actions.Add(new ActionConfig(ActionStateType.ChaseMove));
        actions.Add(new ActionConfig(ActionStateType.StayAtRangeMove));
        actions.Add(new ActionConfig(ActionStateType.FleeMove));
        actions.Add(new ActionConfig(ActionStateType.TurnAroundMove));
        actions.Add(new ActionConfig(ActionStateType.MeleeAttack));
        actions.Add(new ActionConfig(ActionStateType.RangedAttack));
        actions.Add(new ActionConfig(ActionStateType.ChargeAttack));
    }
}
