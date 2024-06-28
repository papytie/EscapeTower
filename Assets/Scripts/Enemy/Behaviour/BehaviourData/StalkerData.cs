using System;
using System.Collections.Generic;

[Serializable]
public class StalkerData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;

    public List<ActionConfig> actions = new();

    public StalkerData()
    {
        actions.Add(new ActionConfig(ActionStateType.WaitMove));
        actions.Add(new ActionConfig(ActionStateType.RoamMove));
        actions.Add(new ActionConfig(ActionStateType.ChaseMove));
    }
}
