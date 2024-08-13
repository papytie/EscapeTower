using UnityEngine;

public class AdditiveLayer : MonoBehaviour, IBehaviour
{
    public NPCFSM FSM { get => fsm; set { fsm = value; } }

    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    AdditiveLayerData data;

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as AdditiveLayerData;

        //Init Reaction state
        Init_TakeDamageReaction();
        Init_EmptyAction();

        fsm.SetState(data.emptyAction.name);
    }

    public void SetTakeDamageState()
    {
        fsm.SetState(data.takeDamage.name);
    }


    //------------------------------\---/-------------------------------|
    //----------------------------|ACTIONS|-----------------------------|
    //------------------------------/---\-------------------------------|


    void Init_TakeDamageReaction()
    {
        NPCState state = fsm.GetState(data.takeDamage.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };

        state.OnStateUpdate += () =>
        {
            if (state.Action.IsCompleted)
                fsm.SetState(data.emptyAction.name);
        };
    }

    void Init_EmptyAction()
    {
        NPCState state = fsm.GetState(data.emptyAction.name);

        state.OnStateEnter += () => { /* First Method called when enter State */ };
        state.OnStateExit += () => { /* Last Method called when exit State */ };
        state.OnStateUpdate += () => { };
    }

    public void SetDieState()
    {
        throw new System.NotImplementedException();
    }
}
