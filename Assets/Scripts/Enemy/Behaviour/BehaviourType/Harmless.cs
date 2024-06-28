using UnityEngine;

public class Harmless : MonoBehaviour, IBehaviour
{
    public NPCFSM FSM => fsm;
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;

    public void InitBehaviour(EnemyController enemyController)
    {
        //Get EnemyController ref
        controller = enemyController;

        //Instantiate FSM
        fsm = new NPCFSM();

        //Instantiate each state in NPCFSM from Controller 
        foreach (var item in controller.EnemyActions)
        {
            fsm.AddState(new NPCState(fsm, item.Key, item.Value));
        }

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_ChaseState();

        //Set this Behaviour starting default state
        fsm.SetState(ActionStateType.WaitMove);
    }

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(ActionStateType.WaitMove);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.StateAction.IsCompleted)
            {
                if (!controller.TargetAcquired)
                    fsm.SetState(ActionStateType.RoamMove);

                if (controller.TargetAcquired && !controller.InAttackRange)
                    fsm.SetState(ActionStateType.ChaseMove);

            }
        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(ActionStateType.RoamMove);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
                fsm.SetState(ActionStateType.ChaseMove);

            if (!controller.TargetAcquired && controller.EnemyActions[ActionStateType.RoamMove].IsCompleted)
                fsm.SetState(ActionStateType.WaitMove);

        };

    }

    void Init_ChaseState()
    {
        NPCState state = fsm.GetState(ActionStateType.ChaseMove);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (!controller.TargetAcquired)
                fsm.SetState(ActionStateType.WaitMove);

        };
    }
}
