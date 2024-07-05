using System;
using System.Collections.Generic;

[Serializable]
public class StalkerData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;
    public List<ActionConfig> actions;

    public StalkerData()
    {
        actions = new List<ActionConfig>
        { 
            { new ActionConfig(ActionType.WaitMove, StalkerActionID.WAIT) },
            { new ActionConfig(ActionType.RoamMove, StalkerActionID.ROAM) },
            { new ActionConfig(ActionType.ChaseMove, StalkerActionID.CHASE) },
            { new ActionConfig(ActionType.ChargeAttack, StalkerActionID.CHARGE) },
            { new ActionConfig(ActionType.TakeDamageReaction, ReactionID.TAKEDMG) },
            { new ActionConfig(ActionType.DieReaction, ReactionID.DIE) }
        };

    }
}

static class StalkerActionID
{
    public static string WAIT = "StalkerWait";
    public static string ROAM = "StalkerRoam";
    public static string CHASE = "StalkerChase";
    public static string CHARGE = "StalkerCharge";
}
