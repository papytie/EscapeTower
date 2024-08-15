using System;
using System.Collections.Generic;
using UnityEngine;
using static BulletHellData;

public class BulletHell : MonoBehaviour, IBehaviour
{
    public NPCFSM FSM { get => fsm; set { fsm = value; } }
    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    BulletHellData data;

    bool UltimateReady => fsm.GetState(data.galaxyUltimate.name).Action.IsAvailable && fsm.GetState(data.beamUltimate.name).Action.IsAvailable && fsm.GetState(data.novaUltimate.name).Action.IsAvailable;

    List<NPCState> shotsList = new();
    List<NPCState> ultimatesList = new();

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as BulletHellData;

        //Init Reaction state
        Init_DieReaction();

        //Customize each Init state
        Init_WaitState();
        Init_AttackSelectionState();
        Init_RoamState();
        Init_SingleShotState();
        Init_SuperShotState();
        Init_MultiShotState();
        Init_SpreadShotState();
        Init_NovaUltimateState();
        Init_BeamUltimateState();
        Init_GalaxyUltimateState();

        //Set this Behaviour starting default state
        fsm.SetState(data.wait.name);
    }

    public void SetTakeDamageState()
    {
        controller.AdditiveBehaviour.SetTakeDamageState();
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
                if (UltimateReady)
                    SetRandomState(ultimatesList);
                else 
                    SetRandomState(shotsList);
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

    void Init_SingleShotState()
    {
        NPCState state = fsm.GetState(data.singleShot.name);
        shotsList.Add(state);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);
        };
        
        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_SuperShotState()
    {
        NPCState state = fsm.GetState(data.superShot.name);
        shotsList.Add(state);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);
        };
        
        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_MultiShotState()
    {
        NPCState state = fsm.GetState(data.multiShot.name);
        shotsList.Add(state);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);
        };
        
        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_SpreadShotState()
    {
        NPCState state = fsm.GetState(data.spreadShot.name);
        shotsList.Add(state);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);
        };
        
        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_BeamUltimateState()
    {
        NPCState state = fsm.GetState(data.beamUltimate.name);
        ultimatesList.Add(state);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);
        };
        
        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_NovaUltimateState()
    {
        NPCState state = fsm.GetState(data.novaUltimate.name);
        ultimatesList.Add(state);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);
        };
        
        state.OnStateExit += () => { /* Last Method called when exit State */ };
    }

    void Init_GalaxyUltimateState()
    {
        NPCState state = fsm.GetState(data.galaxyUltimate.name);
        ultimatesList.Add(state);

        state.OnStateEnter += () => { /* First Method called when enter State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted || !controller.TargetAcquired)
                fsm.SetState(data.wait.name);
        };

        state.OnStateExit += () => { /* Last Method called when exit State */ };
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

    //------------------------------\---/--------------------------------|
    //----------------------------|HELPERS|------------------------------|
    //------------------------------/---\--------------------------------|

    void SetRandomState(List<NPCState> list)
    {
        List<NPCState> availableState = new();

        foreach (var shot in list)
            if(shot.Action.IsAvailable)
                availableState.Add(shot);
        
        if (availableState.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableState.Count);
            fsm.SetState(availableState[randomIndex].ID);
        }
    }
}
