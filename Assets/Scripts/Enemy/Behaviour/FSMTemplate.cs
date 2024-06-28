using System;
using System.Collections.Generic;

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
    public NPCFSM FSM { get; set; }
    public ActionStateType State { get; set; }
    public IAction StateAction { get; set; }

    public NPCState(NPCFSM fsm, ActionStateType type, IAction action)
    {
        FSM = fsm;
        State = type;
        StateAction = action;
    }

    public event Action OnStateEnter = null;
    public event Action OnStateExit = null;
    public event Action OnStateUpdate = null;

    public void StateEnter()
    {
        //Debug.Log("Enter " + State.ToString());
        OnStateEnter?.Invoke();
        StateAction?.StartProcess();
    }

    public void StateExit()
    {
        //Debug.Log("Exit " + State.ToString());
        StateAction?.EndProcess();
        OnStateExit?.Invoke();
    }

    public void Update()
    {
        //Debug.Log("Update " + State.ToString());
        StateAction?.UpdateProcess();
        OnStateUpdate?.Invoke();
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

    public void SetState(ActionStateType type) 
    {
        CurrentState?.StateExit();
        CurrentState = GetState(type);
        CurrentState?.StateEnter();
    }

    public NPCState GetState(ActionStateType type) 
    {
        foreach (var item in StateList)
        {
            if (item.State == type) return item;
        };
        return null;
    }
}
