using UnityEngine;

public class WaitAction : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted { get; set; }

    WaitData data;
    EnemyController controller;

    float processEndTime;
    Vector2 direction;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as WaitData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        IsCompleted = false;
        direction = Vector2.zero;
        processEndTime = Time.time + Random.Range(data.minTime, data.maxTime);
        
        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed/2);
    }

    public void UpdateProcess()
    {
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
        
        if (Time.time >= processEndTime)
        {
            IsCompleted = true;
        }
    }

    public void EndProcess()
    {
    }
}
