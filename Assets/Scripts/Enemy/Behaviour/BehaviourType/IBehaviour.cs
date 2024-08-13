public interface IBehaviour
{
    NPCFSM FSM { get; set; }
    EnemyController Controller { get; }
    void Init(EnemyController enemyController, IBehaviourData behaviourData);
    void SetTakeDamageState();
    void SetDieState();
}