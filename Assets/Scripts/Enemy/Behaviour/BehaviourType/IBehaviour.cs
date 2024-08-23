public interface IBehaviour
{
    NPCFSM FSM { get; set; }
    EnemyController Controller { get; }
    IBehaviourData Data { get; }
    void Init(EnemyController enemyController, IBehaviourData behaviourData);
    void UpdateFSM();
}