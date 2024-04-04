using System;
using UnityEngine;

public interface IMovement
{
    public Vector2 EnemyDirection { get; set; }
    public void Init(IMovementData data);
    public void Move(GameObject target, EnemyCollisionComponent collision);
}