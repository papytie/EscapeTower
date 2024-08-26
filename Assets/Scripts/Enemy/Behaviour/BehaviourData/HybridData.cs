using System;
using System.Collections.Generic;

[Serializable]
public class HybridData : IBehaviourData
{
    public List<ActionConfig> Actions { get => actions; set { actions = value; } }
    List<ActionConfig> actions;

    public ActionConfig waitAction;
    public ActionConfig defaultMove;
    public ActionConfig mainMove;
    public ActionConfig takeDamage;
    public ActionConfig die;

    public List<ActionConfig> attacks;
    public bool randomAttacks;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            waitAction, defaultMove, mainMove, takeDamage, die
        };

        foreach (var attacks in attacks) { actions.Add(attacks); }
    }

}