using System;
using UnityEngine;

public interface IMovement
{
    public Vector2 EnemyDirection { get; set; }
    public Animator EnemyAnimator { get; set; }
    public void Init(IMovementData data, Animator animator);
    public void Move(GameObject target, EnemyCollisionComponent collision);
}