using UnityEngine;

public class Stalker : MonoBehaviour, IBehaviour
{
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    StalkerData data;

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as StalkerData;

        //Init Reaction state
        Init_TakeDamageReaction();
        Init_DieReaction();

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_ChaseState();
        Init_ChargeAttack();

        //Set this Behaviour starting default state
        fsm.SetState(data.wait.name);
    }

    public void SetTakeDamageState()
    {
        fsm.SetState(data.takeDamage.name);
    }

    public void SetDieState()
    {
        fsm.SetState(data.die.name);
    }

    //------------------------------\---/-------------------------------|
    //----------------------------|ACTIONS|-----------------------------|
    //------------------------------/---\-------------------------------|

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(data.wait.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
            {
                if (fsm.GetState(data.charge.name).Action.IsAvailable)
                    fsm.SetState(data.charge.name);

                else if (fsm.GetState(data.chase.name).Action.IsAvailable)
                    fsm.SetState(data.chase.name);
            }
            else if (state.Action.IsCompleted)
            {
                fsm.SetState(data.roam.name);

            }
        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(data.roam.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired)
            {
                if (fsm.GetState(data.chase.name).Action.IsAvailable)
                    fsm.SetState(data.chase.name);

            }
            else if (state.Action.IsCompleted)
            {
                fsm.SetState(data.wait.name);

            }
        };
    }

    void Init_ChaseState()
    {
        NPCState state = fsm.GetState(data.chase.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (controller.TargetAcquired && fsm.GetState(data.charge.name).Action.IsAvailable)
            {
                fsm.SetState(data.charge.name);

            }
            else if (state.Action.IsCompleted || !controller.TargetAcquired)
            {
                fsm.SetState(data.wait.name);

            }
        };
    }

    void Init_ChargeAttack()
    {
        NPCState state = fsm.GetState(data.charge.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.wait.name);
        };
    }

    //-------------------------------\---/-------------------------------|
    //----------------------------|REACTIONS|----------------------------|
    //-------------------------------/---\-------------------------------|

    void Init_TakeDamageReaction()
    {
        NPCState state = fsm.GetState(data.takeDamage.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.wait.name);
        };
    }

    void Init_DieReaction()
    {
        NPCState state = fsm.GetState(data.die.name);

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
