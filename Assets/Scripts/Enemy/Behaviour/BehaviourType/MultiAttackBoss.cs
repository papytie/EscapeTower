using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttackBoss : MonoBehaviour, IBehaviour
{
    public IBehaviourData Data => data;
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    MultiAttackBossData data;

    AdditiveBehaviour additiveBehaviour;

    List<NPCState> shotsList = new();
    List<NPCState> ultimatesList = new();

    bool isValid = true;

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as MultiAttackBossData;

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
                return; 
            }
        }

        //Instantiate and Init Additive Behaviour 
        data.additiveBehaviourData.InitActionsList();
        additiveBehaviour = controller.gameObject.AddComponent<AdditiveBehaviour>();
        additiveBehaviour.Init(controller, data.additiveBehaviourData);

        //Init Reaction state
        Init_DieReaction();

        //Subscribe dieState to OnDeath event
        controller.LifeSystem.OnDeath += () =>
        {
            fsm.SetState(data.die.name);
        };

        //Customize each Init state
        Init_WaitState();
        Init_AttackSelectionState();
        Init_RoamState();
        Init_ShotStates();
        Init_UltimateStates();

        //Set this Behaviour starting default state
        fsm.SetState(data.wait.name);
    }

    public void UpdateFSM()
    {
        if(isValid)
        {
            FSM.CurrentState.Update();
            additiveBehaviour.UpdateFSM();
        }
    }

    //------------------------------\---/-------------------------------|
    //----------------------------|ACTIONS|-----------------------------|
    //------------------------------/---\-------------------------------|

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(data.wait.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.roam.name);

            if(controller.TargetAcquired)
                fsm.SetState(data.attackSelection.name);
        };

        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_AttackSelectionState()
    {
        NPCState state = fsm.GetState(data.attackSelection.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);

            else 
            {
                if (fsm.IsAllActionsAvailable(ultimatesList))
                    fsm.SetRandomState(ultimatesList);
                else if (fsm.IsAnyActionAvailable(shotsList))
                    fsm.SetRandomState(shotsList);
            }
        };

        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(data.roam.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.wait.name);

            if (controller.TargetAcquired)
                fsm.SetState(data.attackSelection.name);
        };

        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_ShotStates()
    {
        foreach (var shot in data.shots)
        {
            NPCState state = fsm.GetState(shot.name);
            shotsList.Add(state);

            state.OnStateEnter += () => { /* First Method called when enter State */ };

            state.OnStateUpdate += () =>
            {
                if (state.Action.IsCompleted /*|| !controller.TargetAcquired*/)
                    fsm.SetState(data.wait.name);
            };
        
            state.OnStateExit += () => { /* Last Method called when exit State */ };
        }
    }

    void Init_UltimateStates()
    {
        foreach (var ultimate in data.ultimates)
        {
            NPCState state = fsm.GetState(ultimate.name);
            ultimatesList.Add(state);

            state.OnStateEnter += () => { /* First Method called when enter State */ };

            state.OnStateUpdate += () =>
            {
                if (state.Action.IsCompleted /*|| !controller.TargetAcquired*/)
                    fsm.SetState(data.wait.name);
            };

            state.OnStateExit += () => { /* Last Method called when exit State */ };
        }
    }

    //-------------------------------\---/-------------------------------|
    //----------------------------|REACTIONS|----------------------------|
    //-------------------------------/---\-------------------------------|

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
