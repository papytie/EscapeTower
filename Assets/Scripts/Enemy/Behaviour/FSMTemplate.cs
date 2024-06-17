using System;
using System.Collections.Generic;

public interface IState
{
    StateType State { get; set; }
    NPCFSM FSM {get; set;}

    void InitState(NPCFSM fsm, StateType type) 
    {
        FSM = fsm;
        State = type;
    }

    void StateEnter() { }
    void StateExit() { }
    void Update() { }
}

public class NPCState : IState
{
    public NPCFSM FSM { get; set; }
    public StateType State { get; set; }

    public NPCState(NPCFSM fsm, StateType type)
    {
        FSM = fsm;
        State = type;
    }

    public event Action OnEnter = null;
    public event Action OnExit = null;
    public event Action OnUpdate = null;

    public void StateEnter()
    {
        OnEnter?.Invoke();
    }

    public void StateExit()
    {
        OnExit?.Invoke();
    }

    public void Update()
    {
        OnUpdate?.Invoke();
    }
}

public class NPCFSM
{
    public List<NPCState> StateList { get; set; }
    public IState CurrentState { get; set; }

    public NPCFSM()
    {
        StateList = new List<NPCState>();
        CurrentState = null;
    }

    public void AddState(NPCState state)
    {
        StateList.Add(state);
    }

    public void SetState(StateType type) 
    {
        CurrentState?.StateExit();
        CurrentState = GetState(type);
        CurrentState?.StateEnter();
    }

    public NPCState GetState(StateType type) 
    {
        foreach (var item in StateList)
        {
            if (item.State == type) return item;
        };
        return null;
    }
}
