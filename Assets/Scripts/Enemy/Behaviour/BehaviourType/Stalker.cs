using UnityEngine;

public class Stalker : MonoBehaviour, IBehaviour
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
        Init_ChaseState();
        Init_ChargeAttack();

        //Set this Behaviour starting default state
        fsm.SetState(StalkerActionID.WAIT);
    }

    //------------------------------\---/-------------------------------|
    //----------------------------|ACTIONS|-----------------------------|
    //------------------------------/---\-------------------------------|

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(StalkerActionID.WAIT);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
            {
                if (fsm.GetState(StalkerActionID.CHARGE).Action.IsAvailable)
                    fsm.SetState(StalkerActionID.CHARGE);

                else if (fsm.GetState(StalkerActionID.CHASE).Action.IsAvailable)
                    fsm.SetState(StalkerActionID.CHASE);
            }
            else if (state.Action.IsCompleted)
            {
                fsm.SetState(StalkerActionID.ROAM);

            }
        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(StalkerActionID.ROAM);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
            {
                if (fsm.GetState(StalkerActionID.CHASE).Action.IsAvailable)
                    fsm.SetState(StalkerActionID.CHASE);

            }
            else if (state.Action.IsCompleted)
            {
                fsm.SetState(StalkerActionID.WAIT);

            }
        };
    }

    void Init_ChaseState()
    {
        NPCState state = fsm.GetState(StalkerActionID.CHASE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired && fsm.GetState(StalkerActionID.CHARGE).Action.IsAvailable)
            {
                fsm.SetState(StalkerActionID.CHARGE);

            }
            else if (state.Action.IsCompleted || !controller.TargetAcquired)
            {
                fsm.SetState(StalkerActionID.WAIT);

            }
        };
    }

    void Init_ChargeAttack()
    {
        NPCState state = fsm.GetState(StalkerActionID.CHARGE);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(StalkerActionID.WAIT);
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
                fsm.SetState(StalkerActionID.WAIT);
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
