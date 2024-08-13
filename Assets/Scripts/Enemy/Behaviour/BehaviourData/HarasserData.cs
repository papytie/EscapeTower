using System;
using System.Collections.Generic;

[Serializable]
public class HarasserData : IBehaviourData
{
    public List<ActionConfig> Actions { get => actions; set { actions = value; } }
    public List<ActionConfig> actions;

    public ActionConfig wait;
    public ActionConfig roam;
    public ActionConfig stayAtRange;
    public ActionConfig ranged;
    public ActionConfig takeDamage;
    public ActionConfig die;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            wait, roam, stayAtRange, ranged, takeDamage, die
        };
    }
}