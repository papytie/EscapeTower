using System;
using System.Collections.Generic;
using UnityEngine;

public class KnightBehaviour : MonoBehaviour, IBehaviour
{
    //TODO : This script only manage to set state selection in the right sequence

    NPCFSM fsm;

    private void Update()
    {
        fsm.CurrentState.Update();
    }

    public void InitBehaviour()
    {

        //TODO : Instantiate Attacks and Moves Scripts here

        //Instantiate FSM
        fsm = new NPCFSM();

        //Instantiate each state
        fsm.AddState(new NPCState(fsm, StateType.Wait));
        fsm.AddState(new NPCState(fsm, StateType.Roam));
        fsm.AddState(new NPCState(fsm, StateType.Detection));
        fsm.AddState(new NPCState(fsm, StateType.Chase));
        fsm.AddState(new NPCState(fsm, StateType.Attack));

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_DetectionState();
        Init_ChaseState();
        Init_AttackState();

        fsm.SetState(StateType.Wait);
    }

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(StateType.Wait);

        state.OnEnter += () =>
        {
            Debug.Log("Enter in " + state.State.ToString() + " State");
        };

        state.OnExit += () =>
        {
            Debug.Log("Exit " + state.State.ToString() + " State");
        };

        state.OnUpdate += () =>
        {
            Debug.Log("Update " + state.State.ToString() + " State");

            //TODO: SET THE LOGIC

        };
    }

    void Init_ChaseState()
    {
        NPCState state = fsm.GetState(StateType.Chase);

        state.OnEnter += () =>
        {
            Debug.Log("Enter in " + state.State.ToString() + " State");
        };

        state.OnExit += () =>
        {
            Debug.Log("Exit " + state.State.ToString() + " State");
        };

        state.OnUpdate += () =>
        {
            Debug.Log("Update " + state.State.ToString() + " State");

            //TODO: SET THE LOGIC

        };

    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(StateType.Roam);

        state.OnEnter += () =>
        {
            Debug.Log("Enter in " + state.State.ToString() + " State");
        };

        state.OnExit += () =>
        {
            Debug.Log("Exit " + state.State.ToString() + " State");
        };

        state.OnUpdate += () =>
        {
            Debug.Log("Update " + state.State.ToString() + " State");

            //TODO: SET THE LOGIC

        };

    }

    void Init_DetectionState()
    {
        NPCState state = fsm.GetState(StateType.Detection);

        state.OnEnter += () =>
        {
            Debug.Log("Enter in " + state.State.ToString() + " State");
        };

        state.OnExit += () =>
        {
            Debug.Log("Exit " + state.State.ToString() + " State");
        };

        state.OnUpdate += () =>
        {
            Debug.Log("Update " + state.State.ToString() + " State");

            //TODO: SET THE LOGIC

        };

    }

    void Init_AttackState()
    {
        NPCState state = fsm.GetState(StateType.Attack);

        state.OnEnter += () =>
        {
            Debug.Log("Enter in  " + state.State.ToString() + " State");
        };

        state.OnExit += () =>
        {
            Debug.Log("Exit " + state.State.ToString() + " State");
        };

        state.OnUpdate += () =>
        {
            Debug.Log("Update " + state.State.ToString() + " State");

            //TODO: SET THE LOGIC

        };

    }

}
