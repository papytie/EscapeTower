using System;
using UnityEngine;

[Serializable]
public class KnightBehaviour : MonoBehaviour, IBehaviour
{
    public StateType CurrentState => currentState;
    public NPCFSM FSM => fsm;

    StateType currentState;
    NPCFSM fsm;
    EnemyController controller;
    KnightBehaviourData data;

    float waitTime = 0;
    float roamTime = 0;
    float startTime = 0;

    public void Update()
    {
        fsm.CurrentState.Update();
    }

    public void InitBehaviour(IBehaviourData behaviourData, EnemyController enemyController)
    {
        data = (KnightBehaviourData)behaviourData;
        controller = enemyController;

        //Instantiate FSM
        fsm = new NPCFSM();

        //Instantiate each state
        fsm.AddState(new NPCState(fsm, StateType.Wait));
        fsm.AddState(new NPCState(fsm, StateType.Roam));
        fsm.AddState(new NPCState(fsm, StateType.Chase));
        fsm.AddState(new NPCState(fsm, StateType.Charge));

        //Customize each Init state
        Init_WaitState();
        Init_RoamState();
        Init_ChaseState();
        Init_AttackState();

        fsm.SetState(StateType.Wait);
    }

    void Init_WaitState()
    {
        NPCState state = fsm.GetState(StateType.Wait);

        state.OnEnter += () =>
        {
            controller.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed/2);
            currentState = StateType.Wait;
            startTime = Time.time;
            waitTime = UnityEngine.Random.Range(.1f, 2f);
        };

        state.OnExit += () =>
        {
        };

        state.OnUpdate += () =>
        {

            if (Time.time >= startTime + waitTime)
            {
                if (!controller.TargetAcquired)
                    fsm.SetState(StateType.Roam);

                if(controller.TargetAcquired && !controller.InAttackRange)
                    fsm.SetState(StateType.Chase);

                if(controller.TargetAcquired && controller.InAttackRange && !controller.AttackOnCD)
                    fsm.SetState(StateType.Charge);
            }
        };
    }

    void Init_RoamState()
    {
        NPCState state = fsm.GetState(StateType.Roam);

        state.OnEnter += () =>
        {
            currentState = StateType.Roam;
            controller.EnemyMoves[MovementType.Roam].InitMove();
        };

        state.OnExit += () =>
        {
        };

        state.OnUpdate += () =>
        {
            controller.EnemyMoves[MovementType.Roam].Move();
            controller.UpdateMoveAnimDirection(controller.EnemyMoves[MovementType.Roam].EnemyDirection);

            if (controller.TargetAcquired)
                fsm.SetState(StateType.Chase);

            if(!controller.TargetAcquired && controller.EnemyMoves[MovementType.Roam].MoveCompleted)
                fsm.SetState(StateType.Wait);
        };

    }

    void Init_ChaseState()
    {
        NPCState state = fsm.GetState(StateType.Chase);

        state.OnEnter += () =>
        {
            currentState = StateType.Chase;
            Debug.Log("Enter in " + state.State.ToString() + " State");
        };

        state.OnExit += () =>
        {
            Debug.Log("Exit " + state.State.ToString() + " State");
        };

        state.OnUpdate += () =>
        {
            Debug.Log("Update " + state.State.ToString() + " State");

            if(controller.InAttackRange && !controller.AttackOnCD)
                fsm.SetState(StateType.Charge);
            
            if(!controller.TargetAcquired)
                fsm.SetState(StateType.Roam);
        };

    }

    void Init_AttackState()
    {
        NPCState state = fsm.GetState(StateType.Charge);

        state.OnEnter += () =>
        {
            currentState = StateType.Charge;
            Debug.Log("Enter in  " + state.State.ToString() + " State");
        };

        state.OnExit += () =>
        {
            Debug.Log("Exit " + state.State.ToString() + " State");
        };

        state.OnUpdate += () =>
        {
            Debug.Log("Update " + state.State.ToString() + " State");

            if(!controller.TargetAcquired)
                fsm.SetState(StateType.Roam);
            
            if (controller.AttackOnCD)
                fsm.SetState(StateType.Wait);

            if(!controller.InAttackRange)
                fsm.SetState(StateType.Chase);
        };

    }

}
