using System.Collections.Generic;

public interface IBehaviourData
{
    public List<ActionConfig> Actions { get; }
    void InitConfigList() { }
}
