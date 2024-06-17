using UnityEngine;

public interface IMovement
{
    public Vector2 EnemyDirection { get; set; }
    public void Init(IMovementData data);
    public void Move(GameObject target, CollisionCheckerComponent collision, CircleCollider2D collider, float moveSpeed);
}