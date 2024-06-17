using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    NPCFSM fsm;

    private void Start()
    {
        InitFSM();
    }

    private void Update()
    {
        fsm.CurrentState.Update();
    }

    public void InitFSM()
    {
        Debug.Log("FSM Initialisation");

        //Instantiate FSM
        fsm = new NPCFSM();

        //Instantiate each state
        fsm.AddState(new NPCState(fsm, StateType.Wait));
        fsm.AddState(new NPCState(fsm, StateType.ChaseMove));
        fsm.AddState(new NPCState(fsm, StateType.FleeMove));
        fsm.AddState(new NPCState(fsm, StateType.StayAtRangeMove));
        fsm.AddState(new NPCState(fsm, StateType.MeleeAttack));
        fsm.AddState(new NPCState(fsm, StateType.RangedAttack));

        //Customize each Init state
        Init_WaitState();
        Init_ChaseState();
        Init_FleeState();
        Init_StayAtRangeState();
        Init_MeleeAttackState();
        Init_RangedAttackState();

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

            if (Input.GetKeyDown(KeyCode.C))
            {
                fsm.SetState(StateType.ChaseMove);
            }

            else if (Input.GetKeyDown(KeyCode.F))
            {
                fsm.SetState(StateType.FleeMove);
            }

            else if (Input.GetKeyDown(KeyCode.S))
            {
                fsm.SetState(StateType.StayAtRangeMove);
            }
        };
    }

    void Init_ChaseState()
    {
        NPCState state = fsm.GetState(StateType.ChaseMove);

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

            if (Input.GetKeyDown(KeyCode.W))
            {
                fsm.SetState(StateType.Wait);
            }

            else if (Input.GetKeyDown(KeyCode.F))
            {
                fsm.SetState(StateType.FleeMove);
            }

            else if (Input.GetKeyDown(KeyCode.S))
            {
                fsm.SetState(StateType.StayAtRangeMove);
            }

        };

    }

    void Init_FleeState()
    {
        NPCState state = fsm.GetState(StateType.FleeMove);

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

            if (Input.GetKeyDown(KeyCode.C))
            {
                fsm.SetState(StateType.ChaseMove);
            }

            else if (Input.GetKeyDown(KeyCode.W))
            {
                fsm.SetState(StateType.Wait);
            }

            else if (Input.GetKeyDown(KeyCode.S))
            {
                fsm.SetState(StateType.StayAtRangeMove);
            }

        };

    }

    void Init_StayAtRangeState()
    {
        NPCState state = fsm.GetState(StateType.StayAtRangeMove);

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

            if (Input.GetKeyDown(KeyCode.C))
            {
                fsm.SetState(StateType.ChaseMove);
            }

            else if (Input.GetKeyDown(KeyCode.F))
            {
                fsm.SetState(StateType.FleeMove);
            }

            else if (Input.GetKeyDown(KeyCode.W))
            {
                fsm.SetState(StateType.Wait);
            }

        };

    }

    void Init_MeleeAttackState()
    {
        NPCState state = fsm.GetState(StateType.MeleeAttack);

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
        };

    }

    void Init_RangedAttackState()
    {
        NPCState state = fsm.GetState(StateType.RangedAttack);

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
        };
    }
}
