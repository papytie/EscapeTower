using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovementConfig
{
    public MovementType type = MovementType.Wait;
    [SerializeReference] public IMovementData data;
    public List<Prerequisite> prerequisite = new();
    public int priority;
    public float behaviourRange;
    public EnemyAttackConfig attackConfig;
}