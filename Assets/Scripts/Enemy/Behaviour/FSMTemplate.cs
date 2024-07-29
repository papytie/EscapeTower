using System;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    NPCFSM FSM {get; set;}

    void InitState(NPCFSM fsm) 
    {
        FSM = fsm;
    }

    void StateEnter() { }
    void StateExit() { }
    void Update() { }
}

public class NPCState : IState
{
    public string ID { get; set; }
    public NPCFSM FSM { get; set; }
    public IAction Action { get; set; }

    public event Action OnStateEnter = null;
    public event Action OnStateExit = null;
    public event Action OnStateUpdate = null;

    public NPCState(NPCFSM fsm, string stateID, IAction action)
    {
        FSM = fsm;
        ID = stateID;
        Action = action;
    }

    public void StateEnter()
    {
        Debug.Log("Enter " + Action.ToString());
        OnStateEnter?.Invoke();
        Action?.StartProcess();
    }

    public void StateExit()
    {
        Debug.Log("Exit " + Action.ToString());
        Action?.EndProcess();
        OnStateExit?.Invoke();
    }

    public void Update()
    {
        //Debug.Log("Update " + Action.ToString());
        Action?.UpdateProcess();
        OnStateUpdate?.Invoke();
    }
}

public class NPCFSM
{
    public List<NPCState> StateList { get; set; }
    public NPCState CurrentState { get; set; }

    public NPCFSM()
    {
        StateList = new List<NPCState>();
        CurrentState = null;
    }

    public void AddState(NPCState state)
    {
        StateList.Add(state);
    }

    public void SetState(string stateID) 
    {
        CurrentState?.StateExit();
        CurrentState = GetState(stateID);
        CurrentState?.StateEnter();
    }

    public NPCState GetState(string stateID) 
    {
        foreach (NPCState state in StateList)
        {
            if (state.ID == stateID) return state;
        };
        return null;
    }
}
