using UnityEngine;

public class DieReaction : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;

    Vector2 direction = Vector2.zero;
    DieData data;
    EnemyController controller;
    float processEndTime;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as DieData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        IsCompleted = false;
        processEndTime = Time.time + data.despawnTime;
        controller.AnimationParam.ActivateDieTrigger();
    }

    public void UpdateProcess()
    {
        if(Time.time >= processEndTime)
            IsCompleted = true;
    }

    public void EndProcess()
    {

    }
}
