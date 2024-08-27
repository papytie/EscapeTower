using UnityEngine;

public interface IAction
{
    bool IsAvailable { get; }
    bool IsCompleted { get; }
    Vector3 Direction { get; }
    public void Init(IActionData dataRef, EnemyController controllerRef);
    void StartProcess();
    void UpdateProcess();
    void EndProcess();
}