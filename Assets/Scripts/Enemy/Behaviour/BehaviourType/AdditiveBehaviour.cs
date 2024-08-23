using UnityEngine;

public class AdditiveBehaviour : MonoBehaviour, IBehaviour
{
    public IBehaviourData Data => data;

    public NPCFSM FSM { get => fsm; set { fsm = value; } }

    public EnemyController Controller => controller;

    NPCFSM fsm;
    EnemyController controller;
    AdditiveBehaviourData data;

    bool isValid = true;

    public void Init(EnemyController enemyController, IBehaviourData behaviourData)
    {
        //Get Controller & Data ref
        controller = enemyController;
        data = behaviourData as AdditiveBehaviourData;

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

        //Init Reaction state
        Init_TakeDamageReaction();

        //Subscribe dieState to OnTakeDamage event
        controller.LifeSystem.OnTakeDamage += () =>
        {
            fsm.SetState(data.takeDamage.name);
        };

        Init_EmptyAction();

        fsm.SetState(data.emptyAction.name);
    }

    public void UpdateFSM()
    {
        if(isValid)
            FSM.CurrentState.Update();
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
}
