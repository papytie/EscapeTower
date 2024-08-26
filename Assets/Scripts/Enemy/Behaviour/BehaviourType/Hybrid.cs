using System.Collections.Generic;
using UnityEngine;

public class Hybrid : MonoBehaviour, IBehaviour
{
    public IBehaviourData Data => data;
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    HybridData data;
    bool isValid = true;

    List<NPCState> attacksList = new();

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as HybridData;

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
        Init_waitActionState();
        Init_DefaultMoveState();
        Init_MainMoveState();
        Init_AttackStates();

        //Set this Behaviour starting default state
        fsm.SetState(data.waitAction.name);
    }

    public void UpdateFSM()
    {
        if (isValid)
            FSM.CurrentState.Update();
    }

    //------------------------------\---/-------------------------------|
    //----------------------------|ACTIONS|-----------------------------|
    //------------------------------/---\-------------------------------|

    void Init_waitActionState()
    {
        NPCState state = fsm.GetState(data.waitAction.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.defaultMove.name);

            if (controller.TargetAcquired)
                fsm.SetState(data.mainMove.name);

        };
    }

    void Init_DefaultMoveState()
    {
        NPCState state = fsm.GetState(data.defaultMove.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.waitAction.name);

            if (controller.TargetAcquired)
                fsm.SetState(data.mainMove.name);
        };
    }

    void Init_MainMoveState()
    {
        NPCState state = fsm.GetState(data.mainMove.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.waitAction.name);

            if (fsm.IsAnyActionAvailable(attacksList))
                fsm.SetRandomState(attacksList);
        };
    }

    void Init_AttackStates()
    {
        foreach (var shot in data.attacks)
        {
            NPCState state = fsm.GetState(shot.name);
            attacksList.Add(state);

            state.OnStateEnter += () => { /* First Method called when enter State */ };
            state.OnStateExit += () => { /* Last Method called when exit State */ };

            state.OnStateUpdate += () =>
            {
                if (state.Action.IsCompleted)
                    fsm.SetState(data.waitAction.name);
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
                fsm.SetState(data.waitAction.name);
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
