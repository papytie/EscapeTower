using UnityEngine;

public class Fighter : MonoBehaviour, IBehaviour
{
    public IBehaviourData Data => data;
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    bool isValid = true;
    NPCFSM fsm;
    EnemyController controller;
    FighterData data;

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as FighterData;

        FSM = new NPCFSM();

        foreach (ActionConfig actionConfig in data.Actions)
        {
            if (actionConfig != null)
            {
                IAction action = ActionFactory.Create(gameObject, actionConfig.actionType);
                action.InitRef(actionConfig.data, controller);
                FSM.AddState(new NPCState(FSM, actionConfig.name, action));
            }
            else
            {
                Debug.LogWarning("ActionConfig is missing in : " + data);
                isValid = false;
                return;
            }
        }

        Init_TakeDamageReaction();
        Init_DieReaction();
        controller.LifeSystem.OnTakeDamage += () => { fsm.SetState(data.takeDamage.name); };
        controller.LifeSystem.OnDeath += () => { fsm.SetState(data.die.name); };

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_ChaseState();
        Init_MeleeAttackState();

        //Set this Behaviour starting default state
        fsm.SetState(data.wait.name);
    }

    public void UpdateFSM()
    {
        if (isValid)
            FSM.CurrentState.Update();
    }

    //------------------------------\---/-------------------------------|
    //----------------------------|ACTIONS|-----------------------------|
    //------------------------------/---\-------------------------------|

    void Init_WaitState()
    {
        NPCState waitState = fsm.GetState(data.wait.name);

        waitState.OnStateEnter += () => { /* First Method called when enter State */ };
        waitState.OnStateExit += () => { /* Last Method called when exit State */ };

        waitState.OnStateUpdate += () =>
        { 
            if (controller.TargetAcquired)
            {
                if (fsm.GetState(data.melee.name).Action.IsAvailable)
                    fsm.SetState(data.melee.name);

                else if(fsm.GetState(data.chase.name).Action.IsAvailable)
                    fsm.SetState(data.chase.name);
            }
            else if (fsm.GetState(data.wait.name).Action.IsCompleted)
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
            if (controller.TargetAcquired && fsm.GetState(data.melee.name).Action.IsAvailable)
            {
                fsm.SetState(data.melee.name);

            }
            else if(state.Action.IsCompleted || !controller.TargetAcquired)
            {
                fsm.SetState(data.wait.name);

            }        
        };
    }

    void Init_MeleeAttackState()
    {
        NPCState state = fsm.GetState(data.melee.name);

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
            if(state.Action.IsCompleted)
            {
                controller.LootSystem.RollLoot();
                Destroy(controller.gameObject);
            }
        };
    }
}
