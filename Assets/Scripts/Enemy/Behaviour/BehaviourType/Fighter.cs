using UnityEngine;

public class Fighter : MonoBehaviour, IBehaviour
{
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;

    public void InitBehaviour(EnemyController enemyController)
    {
        //Get EnemyController ref
        controller = enemyController;

        //Init Reaction state
        Init_TakeDamageReaction();
        Init_DieReaction();

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_ChaseState();
        Init_MeleeAttackState();

        //Set this Behaviour starting default state
        fsm.SetState(FighterActionID.WAIT);
    }

    //------------------------------\---/-------------------------------|
    //----------------------------|ACTIONS|-----------------------------|
    //------------------------------/---\-------------------------------|

    void Init_WaitState()
    {
        NPCState waitState = fsm.GetState(FighterActionID.WAIT);

        waitState.OnStateEnter += () => { /* First Method called when enter State */ };
        waitState.OnStateExit += () => { /* Last Method called when exit State */ };

        waitState.OnStateUpdate += () =>
        { 
            if (controller.TargetAcquired)
            {
                if (fsm.GetState(FighterActionID.MELEE).Action.IsAvailable)
                    fsm.SetState(FighterActionID.MELEE);

                else if(fsm.GetState(FighterActionID.CHASE).Action.IsAvailable)
                    fsm.SetState(FighterActionID.CHASE);
            }
            else if (fsm.GetState(FighterActionID.WAIT).Action.IsCompleted)
            {
                fsm.SetState(FighterActionID.ROAM);

            }
        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(FighterActionID.ROAM);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
            {
                if (fsm.GetState(FighterActionID.CHASE).Action.IsAvailable)
                    fsm.SetState(FighterActionID.CHASE);

            }
            else if (state.Action.IsCompleted)
            {
                fsm.SetState(FighterActionID.WAIT);

            }
        };
    }

    void Init_ChaseState()
    {
        NPCState state = fsm.GetState(FighterActionID.CHASE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired && fsm.GetState(FighterActionID.MELEE).Action.IsAvailable)
            {
                fsm.SetState(FighterActionID.MELEE);

            }
            else if(state.Action.IsCompleted || !controller.TargetAcquired)
            {
                fsm.SetState(FighterActionID.WAIT);

            }        
        };
    }

    void Init_MeleeAttackState()
    {
        NPCState state = fsm.GetState(FighterActionID.MELEE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(FighterActionID.WAIT);
        };
    }

    //-------------------------------\---/-------------------------------|
    //----------------------------|REACTIONS|----------------------------|
    //-------------------------------/---\-------------------------------|

    void Init_TakeDamageReaction()
    {
        NPCState state = fsm.GetState(ReactionID.TAKEDMG);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(FighterActionID.WAIT);
        };
    }

    void Init_DieReaction()
    {
        NPCState state = fsm.GetState(ReactionID.DIE);
           
        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () => 
        { 
            if(state.Action.IsCompleted)
            {
                controller.LootSystem.RollLoot();
                Destroy(controller.gameObject);
            }
        };
    }
}
