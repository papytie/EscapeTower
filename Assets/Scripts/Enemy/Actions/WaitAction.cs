using UnityEngine;

public class WaitAction : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    WaitData data;
    EnemyController controller;

    float processEndTime;
    Vector2 direction;

    public void Init(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as WaitData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        IsCompleted = false;

        if (data.minTime != data.maxTime) 
            processEndTime = Time.time + Random.Range(data.minTime, data.maxTime);  
        else processEndTime = Time.time + data.minTime;

        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed / 2);
        controller.AnimationParam.UpdateMoveAnimDirection(controller.CurrentDirection * .1f);
    }

    public void UpdateProcess()
    { 
        if (Time.time >= processEndTime)
        {
            IsCompleted = true;
        }
    }

    public void EndProcess()
    {
        IsCompleted = false;
    }
}
