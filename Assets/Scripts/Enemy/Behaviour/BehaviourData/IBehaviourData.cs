using System.Collections.Generic;

public interface IBehaviourData
{
    public List<ActionConfig> Actions { get; }
}

public static class ReactionID
{
    public static string TAKEDMG = "TakeDamage";
    public static string DIE = "Die";

}
