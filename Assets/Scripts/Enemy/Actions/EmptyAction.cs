using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyAction : MonoBehaviour, IAction
{
    public bool IsAvailable => true;
    public bool IsCompleted => false;
    public Vector3 Direction => direction;

    Vector2 direction;
    EmptyData data;
    EnemyController controller;

    public void InitRef(IActionData dataRef, EnemyController controllerRef) 
    {
        data = dataRef as EmptyData;
        controller = controllerRef;
    }
    public void StartProcess() { }  
    public void UpdateProcess() { }
    public void EndProcess() { }
}
