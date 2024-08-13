using System;
using System.Collections.Generic;

[Serializable]
public class FighterData : IBehaviourData
{
    public List<ActionConfig> Actions { get => actions; set { actions = value; } }
    public List<ActionConfig> actions;

    public ActionConfig wait;
    public ActionConfig roam;
    public ActionConfig chase;
    public ActionConfig melee;
    public ActionConfig takeDamage;
    public ActionConfig die;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            wait, roam, chase, melee, takeDamage, die
        };

    }
}