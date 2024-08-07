using System;
using System.Collections.Generic;

[Serializable]
public class HarasserData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;
    public List<ActionConfig> actions;

    public HarasserData()
    {
        actions = new List<ActionConfig>
        {
            { new ActionConfig(ActionType.WaitMove, HarasserActionID.WAIT) },
            { new ActionConfig(ActionType.RoamMove, HarasserActionID.ROAM) },
            { new ActionConfig(ActionType.StayAtRangeMove, HarasserActionID.STAYATRANGE) },
            { new ActionConfig(ActionType.RangedAttack, HarasserActionID.RANGED) },
            { new ActionConfig(ActionType.TakeDamageReaction, ReactionID.TAKEDMG) },
            { new ActionConfig(ActionType.DieReaction, ReactionID.DIE) }
        };
    }


}
static class HarasserActionID
{
    public static string WAIT = "HarasserWait";
    public static string ROAM = "HarasserRoam";
    public static string STAYATRANGE = "HarasserStayAtRange";
    public static string RANGED = "HarasserRanged";
}
