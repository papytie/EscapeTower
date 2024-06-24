using System.Collections.Generic;

public interface IBehaviourData
{
    public List<AttackConfig> AttacksList { get; }
    public List<MovementConfig> MovesList { get; }
    void InitConfigList() { }
}
