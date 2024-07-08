public interface IBehaviour
{
    NPCFSM FSM { get; set; }
    EnemyController Controller { get; }
    void Init(EnemyController enemyController);
}