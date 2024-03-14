using System;
using UnityEngine;

public interface IMovement
{
    public void Init(IMovementData data);
    public void Move(GameObject target, EnemyCollision collision);
}