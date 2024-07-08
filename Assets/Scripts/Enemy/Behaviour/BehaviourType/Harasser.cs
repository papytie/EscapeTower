using UnityEngine;

public class Harasser : MonoBehaviour, IBehaviour
{
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;

    public void Init(EnemyController enemyController)
    {
        //Get EnemyController ref
        controller = enemyController;

        //Init Reaction state
        Init_TakeDamageReaction();
        Init_DieReaction();

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_StayAtRangeState();
        Init_RangedAttackState();

        //Set this Behaviour starting default state
        fsm.SetState(HarasserActionID.WAIT);
    }

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(HarasserActionID.WAIT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
                fsm.SetState(HarasserActionID.STAYATRANGE);
            
            if (state.Action.IsCompleted)
                fsm.SetState(HarasserActionID.ROAM);

        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(HarasserActionID.ROAM);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
                fsm.SetState(HarasserActionID.STAYATRANGE);

            if (state.Action.IsCompleted)
                fsm.SetState(HarasserActionID.WAIT);

        };

    }

    void Init_StayAtRangeState()
    {
        NPCState state = fsm.GetState(HarasserActionID.STAYATRANGE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (!controller.TargetAcquired)
                fsm.SetState(HarasserActionID.WAIT);

            if(controller.TargetAcquired && fsm.GetState(HarasserActionID.RANGED).Action.IsAvailable)
                fsm.SetState(HarasserActionID.RANGED);
        };
    }

    void Init_RangedAttackState()
    {
        NPCState state = fsm.GetState(HarasserActionID.RANGED);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(HarasserActionID.WAIT);
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
                fsm.SetState(HarasserActionID.WAIT);
        };
    }

    void Init_DieReaction()
    {
        NPCState state = fsm.GetState(ReactionID.DIE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
            {
                controller.LootSystem.RollLoot();
                Destroy(gameObject);
            }
        };
    }

}
