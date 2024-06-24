public interface IBehaviour
{
    public StateType CurrentState { get; }
    public NPCFSM FSM { get; }
    public void Update() { }
    public void InitBehaviour(IBehaviourData data, EnemyController controller) { }
}