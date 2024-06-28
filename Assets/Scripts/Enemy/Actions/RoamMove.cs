using System;
using System.Collections.Generic;
using UnityEngine;

public class RoamMove : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted { get; set; }

    RoamData data;
    EnemyController controller;

    float processEndTime;
    Vector2 direction;

    public void InitRef(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as RoamData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        //Initialise IsCompleted switch to OFF
        IsCompleted = false;

        //Set Random direction
        direction = UnityEngine.Random.insideUnitCircle.normalized;

        //Set TimerEnd
        processEndTime = Time.time + UnityEngine.Random.Range(data.minTime, data.maxTime);

        //Update Animator Param
        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed * data.speedMult);
    }

    public void UpdateProcess()
    {
        //Movement with collisions adjustments
        controller.Collision.MoveToCollisionCheck(direction, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
        transform.position = finalPosition;

        //Update Animator Param
        controller.AnimationParam.UpdateMoveAnimDirection(direction);

        //Reflect movement direction if collision occur
        if(hitList.Count > 0) 
            direction = Vector2.Reflect(direction, hitList[0].normal);

        //End Process with switch to ON
        if (Time.time >= processEndTime)
            IsCompleted = true;
    }

    public void EndProcess()
    {
    }
}