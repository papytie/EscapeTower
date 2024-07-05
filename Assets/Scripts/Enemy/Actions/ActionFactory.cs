using UnityEngine;

public class ActionFactory
{
    public static IAction Create(GameObject thisObject, ActionType actionType)
    {
        return actionType switch
        {
            ActionType.WaitMove => thisObject.AddComponent<WaitAction>(),
            ActionType.ChaseMove => thisObject.AddComponent<ChaseMove>(),
            ActionType.FleeMove => thisObject.AddComponent<FleeMove>(),
            ActionType.StayAtRangeMove => thisObject.AddComponent<StayAtRangeMove>(),
            ActionType.TurnAroundMove => thisObject.AddComponent<TurnAroundMove>(),
            ActionType.RoamMove => thisObject.AddComponent<RoamMove>(),
            ActionType.MeleeAttack => thisObject.AddComponent<MeleeAttack>(),
            ActionType.RangedAttack => thisObject.AddComponent<RangedAttack>(),
            ActionType.ChargeAttack => thisObject.AddComponent<ChargeAttack>(),
            ActionType.TakeDamageReaction => thisObject.AddComponent<TakeDamageReaction>(),
            ActionType.DieReaction => thisObject.AddComponent<DieReaction>(),
            _ => null,
        };
    }
}
