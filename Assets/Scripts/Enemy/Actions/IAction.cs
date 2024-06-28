using UnityEngine;

public interface IAction
{
    bool IsAvailable { get; }
    bool IsCompleted { get; }
    void InitRef(IActionData dataRef, EnemyController controllerRef);
    void StartProcess();
    void UpdateProcess();
    void EndProcess();
}