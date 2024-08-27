using System.Collections.Generic;
using UnityEngine;

public class Stalker : MonoBehaviour, IBehaviour
{
    public IBehaviourData Data => data;
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    StalkerData data;
    bool isValid = true;
    List<NPCState> attackStates = new();

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as StalkerData;

        FSM = new NPCFSM();

        foreach (ActionConfig actionConfig in data.Actions)
        {
            if (actionConfig != null)
            {
                IAction action = ActionFactory.Create(gameObject, actionConfig.actionType);
                action.Init(actionConfig.data, controller);
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
        Init_WaitState();
        Init_RoamState();
        Init_ChaseState();
        Init_ChargeAttackState();
        //Init_DropActionState();
        Init_ShotStates();

        controller.LifeSystem.OnTakeDamage += () => 
        { 
            fsm.SetState(data.takeDamage.name);
        };
        controller.LifeSystem.OnDeath += () => 
        { 
            fsm.SetState(data.die.name); 
        };

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

            if (fsm.IsAnyActionAvailable(attackStates))
                fsm.SetRandomState(attackStates);

            if (state.Action.IsCompleted || !controller.TargetAcquired)
            {
                fsm.SetState(data.wait.name);
            }
        };
    }

    void Init_ChargeAttackState()
    {
        NPCState state = fsm.GetState(data.charge.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if(fsm.IsAnyActionAvailable(attackStates))
                fsm.SetRandomState(attackStates);

            if (state.Action.IsCompleted)
                fsm.SetState(data.wait.name);
        };
    }

    void Init_ShotStates()
    {
        if (data.attacks.Count == 0) return;

        foreach (var shot in data.attacks)
        {
            NPCState state = fsm.GetState(shot.name);
            attackStates.Add(state);

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
