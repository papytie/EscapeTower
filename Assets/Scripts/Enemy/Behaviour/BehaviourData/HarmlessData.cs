using System;
using System.Collections.Generic;

[Serializable]
public class HarmlessData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;
    public List<ActionConfig> actions;

    public HarmlessData()
    {
        actions = new List<ActionConfig>
        {
            { new ActionConfig(ActionType.WaitMove, HarmlessActionID.WAIT) },
            { new ActionConfig(ActionType.RoamMove, HarmlessActionID.ROAM) },
            { new ActionConfig(ActionType.TakeDamageReaction, ReactionID.TAKEDMG) },
            { new ActionConfig(ActionType.DieReaction, ReactionID.DIE) }
        };
    }
}

static class HarmlessActionID
{
    public static string WAIT = "HarmlessWait";
    public static string ROAM = "HarmlessRoam";
}
