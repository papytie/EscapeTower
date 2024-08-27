using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAction : MonoBehaviour, IAction
{
    public bool IsAvailable => Time.time > cooldownEndTime;
    public bool IsCompleted{get;set;}
    public Vector3 Direction => Vector3.zero;

    float cooldownEndTime = 0;
    float startTime = 0;
    bool isDropped = false;
    DropData data;
    EnemyController controller;

    public void EndProcess()
    {
        cooldownEndTime = Time.time;
        isDropped = false;
        IsCompleted = false;
    }

    public void Init(IActionData dataRef, EnemyController controllerRef)
    {
        controller = controllerRef;
        data = dataRef as DropData;
    }

    public void StartProcess()
    {
        startTime = Time.time;
    }

    public void UpdateProcess()
    {
        if(Time.time > startTime + data.reactionTime && !isDropped)
        {
            Vector2 dropPos = controller.transform.position.ToVector2() + data.dropPositionOffset;
            Instantiate(data.dropItem, dropPos, Quaternion.identity);
            isDropped = true;
        }

        if(Time.time > startTime + data.reactionTime + data.duration) 
        { 
            IsCompleted = true;
        }
    }
}
