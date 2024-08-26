using System;
using System.Collections.Generic;

[Serializable]
public class HarmlessData : IBehaviourData
{
    public List<ActionConfig> Actions { get => actions; set { actions = value; } }
    List<ActionConfig> actions;

    public ActionConfig wait;
    public ActionConfig roam;
    public ActionConfig takeDamage;
    public ActionConfig die;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            wait, roam, takeDamage, die
        };
    }
}

