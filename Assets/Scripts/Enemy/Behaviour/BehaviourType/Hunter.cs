using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour, IBehaviour
{
    public IBehaviourData Data => data;
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    HunterData data;
    bool isValid = true;

    List<NPCState> shotsList = new();

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as HunterData;

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
        Init_StayAtRangeState();
        Init_ShotStates();

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
        NPCState state = fsm.GetState(data.wait.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.roam.name);

            if (controller.TargetAcquired)
                fsm.SetState(data.stayAtRange.name);

        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(data.roam.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.wait.name);

            if (controller.TargetAcquired)
                fsm.SetState(data.stayAtRange.name);
        };

    }

    void Init_StayAtRangeState()
    {
        NPCState state = fsm.GetState(data.stayAtRange.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);

            if (fsm.IsAnyActionAvailable(shotsList))
                fsm.SetRandomState(shotsList);
        };
    }

    void Init_ShotStates()
    {
        foreach (var shot in data.shots)
        {
            NPCState state = fsm.GetState(shot.name);
            shotsList.Add(state);

            state.OnStateEnter += () => { /* First Method called when enter State */ };
            state.OnStateExit += () => { /* Last Method called when exit State */ };

            state.OnStateUpdate += () =>
            {
                if (state.Action.IsCompleted)
                    fsm.SetState(data.wait.name);
            };
        }
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
