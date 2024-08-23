using System;
using System.Collections.Generic;

[Serializable]
public class AdditiveBehaviourData : IBehaviourData
{
    public List<ActionConfig> Actions => actions;
    List<ActionConfig> actions;

    public ActionConfig emptyAction;
    public ActionConfig takeDamage;

    public void InitActionsList()
    {
        actions = new List<ActionConfig>
        {
            emptyAction, takeDamage,
        };
    }
}
