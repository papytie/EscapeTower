public interface IBehaviour
{
    
    NPCFSM FSM { get; }
    EnemyController Controller { get; }
    void InitBehaviour(EnemyController enemyController);
    void InitFSM(EnemyController controller)
    {
/*        //Get EnemyController ref
        Controller = controller;

        //Instantiate FSM
        FSM = new NPCFSM();

        //Instantiate each state in NPCFSM from Controller 
        foreach (var item in Controller.EnemyActions)
        {
            FSM.AddState(new NPCState(FSM, item.Key, item.Value));
        }
*/    }
}