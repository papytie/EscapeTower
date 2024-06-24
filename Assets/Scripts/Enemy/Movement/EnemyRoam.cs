using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoam : MonoBehaviour, IMovement
{
    public Vector2 EnemyDirection { get; set; }
    public bool MoveCompleted { get; set; }
    public RoamData Data { get; }

    private RoamData data;
    EnemyController controller;

    private float startTime;
    private float duration;

    public void InitRef(IMovementData dataRef, EnemyController controllerRef)
    {
        data = dataRef as RoamData;
        controller = controllerRef;
    }

    public void InitMove()
    {
        MoveCompleted = false;
        EnemyDirection = UnityEngine.Random.insideUnitCircle.normalized;
        startTime = Time.time;
        duration = UnityEngine.Random.Range(data.minTime, data.maxTime);
        controller.UpdateMoveAnimSpeed(controller.Stats.MoveSpeed * data.speedMult);
    }

    public void Move()
    {
        controller.Collision.MoveToCollisionCheck(EnemyDirection, controller.Stats.MoveSpeed * data.speedMult * Time.deltaTime, controller.Collision.BlockingObjectsLayer, out Vector3 finalPosition, out List<RaycastHit2D> hitList);
        transform.position = finalPosition;

        if (Time.time >= startTime + duration)
        {
            MoveCompleted = true;
        }
        
        if(hitList.Count > 0) 
            EnemyDirection = Vector2.Reflect(EnemyDirection, hitList[0].normal);
    }

}
