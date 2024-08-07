using System;
using System.Collections.Generic;

[Serializable]
public class FighterData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;
    public List<ActionConfig> actions;

    public FighterData()
    {
        actions = new List<ActionConfig>
        {
            { new ActionConfig(ActionType.WaitMove, FighterActionID.WAIT) },
            { new ActionConfig(ActionType.RoamMove, FighterActionID.ROAM) },
            { new ActionConfig(ActionType.ChaseMove, FighterActionID.CHASE) },
            { new ActionConfig(ActionType.MeleeAttack, FighterActionID.MELEE) },
            { new ActionConfig(ActionType.TakeDamageReaction, ReactionID.TAKEDMG) },
            { new ActionConfig(ActionType.DieReaction, ReactionID.DIE) }
        };
    }
}

static class FighterActionID
{
    public static string WAIT = "FighterWait";
    public static string ROAM = "FighterRoam";
    public static string CHASE = "FighterChase";
    public static string MELEE = "FighterMelee";
}
