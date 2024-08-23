using System;
using System.Collections.Generic;

[Serializable]
public class HunterData : IBehaviourData
{
    public List<ActionConfig> Actions { get => actions; set { actions = value; } }
    List<ActionConfig> actions;

    public ActionConfig wait;
    public ActionConfig roam;
    public ActionConfig stayAtRange;
    public ActionConfig takeDamage;
    public ActionConfig die;

    public List<ActionConfig> shots;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            wait, roam, stayAtRange, takeDamage, die
        };

        foreach (var shot in shots) { actions.Add(shot); }
    }

}