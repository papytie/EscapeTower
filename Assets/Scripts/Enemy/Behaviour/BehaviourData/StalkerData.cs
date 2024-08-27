using System;
using System.Collections.Generic;

[Serializable]
public class StalkerData : IBehaviourData
{
    public List<ActionConfig> Actions { get => actions; set { actions = value; } }
    List<ActionConfig> actions;

    public ActionConfig wait;
    public ActionConfig roam;
    public ActionConfig chase;
    public ActionConfig charge;
    public ActionConfig takeDamage;
    public ActionConfig die;

    public List<ActionConfig> attacks;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        { 
            wait, roam, chase, charge, takeDamage, die,
        };

        if (attacks.Count > 0 )
            foreach (var attack in attacks) { actions.Add(attack); }
    }
}