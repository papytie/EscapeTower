using UnityEngine;

public class Harasser : MonoBehaviour, IBehaviour
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
            //Debug.Log("Add new " +  item.Key + (" to NPCFSM"));
        }

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_StayAtRangeState();
        Init_RangedAttackState();

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
            if (controller.TargetAcquired)
                fsm.SetState(ActionStateType.StayAtRangeMove);
            
            if (state.StateAction.IsCompleted)
                fsm.SetState(ActionStateType.RoamMove);

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
                fsm.SetState(ActionStateType.StayAtRangeMove);

            if (state.StateAction.IsCompleted)
                fsm.SetState(ActionStateType.WaitMove);

        };

    }

    void Init_StayAtRangeState()
    {
        NPCState state = fsm.GetState(ActionStateType.StayAtRangeMove);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (!controller.TargetAcquired)
                fsm.SetState(ActionStateType.WaitMove);

            if(controller.TargetAcquired && controller.EnemyActions[ActionStateType.RangedAttack].IsAvailable)
                fsm.SetState(ActionStateType.RangedAttack);
        };
    }

    void Init_RangedAttackState()
    {
        NPCState state = fsm.GetState(ActionStateType.RangedAttack);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.StateAction.IsCompleted)
                fsm.SetState(ActionStateType.WaitMove);
        };
    }
}
