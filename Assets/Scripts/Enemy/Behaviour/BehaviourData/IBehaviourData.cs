using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourData
{
    public List<ActionConfig> Actions { get; }

    public void InitActionsList() { }
}

static class ReactionID
{
    public static string DIE = "Die";
    public static string TAKEDMG = "TakeDamage";
}
