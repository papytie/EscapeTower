using UnityEngine;

public interface IMovement
{
    public bool MoveCompleted { get; set; }
    public Vector2 EnemyDirection { get; set; }
    public void InitRef(IMovementData dataRef, EnemyController controllerRef);
    public void InitMove();
    public void Move();
}