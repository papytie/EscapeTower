using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourData
{
    public List<ActionConfig> Actions { get; }

    public void InitActionsList() { }
}