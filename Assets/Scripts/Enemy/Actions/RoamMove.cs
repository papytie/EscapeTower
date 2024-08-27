using System;
using System.Collections.Generic;
using UnityEngine;

public class RoamMove : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted { get; set; }
    public Vector3 Direction => direction;
    Vector2 direction;

    RoamData data;
    EnemyController controller;
    List<RaycastHit2D> currentHitList = new();

    float processEndTime;

    public void Init(IActionData dataRef, EnemyController controllerRef)
    {
        data = dataRef as RoamData;
        controller = controllerRef;
    }

    public void StartProcess()
    {
        //Set Random direction
        direction = UnityEngine.Random.insideUnitCircle.normalized;

        //Set TimerEnd
        processEndTime = Time.time + UnityEngine.Random.Range(data.minTime, data.maxTime);

        //Update Animator Param
        controller.AnimationParam.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed * data.speedMult);
        controller.AnimationParam.UpdateMoveAnimDirection(direction);
    }

    public void UpdateProcess()
    {
        //Stop Action if too much collision recorded
        if (currentHitList.Count > data.maxBounce)
            IsCompleted = true;

        //End Process with switch to ON
        if (Time.time >= processEndTime)
            IsCompleted = true;

        //Movement with collisions adjustments
        controller.Collision.MoveToCollisionCheck(direction, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
        transform.position = finalPosition;

        //Update Animator Param
        controller.AnimationParam.UpdateMoveAnimDirection(direction);

        //Reflect movement direction if collision occur
        if(hitList.Count > 0)
        {
            direction = Vector2.Reflect(direction, hitList[0].normal);
            foreach (var item in hitList)
            {
                currentHitList.Add(item);
            }
        }
    }

    public void EndProcess()
    {
        //Clear HitList
        currentHitList.Clear();

        IsCompleted = false;

        controller.CurrentDirection = direction;
    }
}
