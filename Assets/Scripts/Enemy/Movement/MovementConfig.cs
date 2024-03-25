using System;
using UnityEngine;

[Serializable]
public class MovementConfig
{
    [SerializeField] public MovementType type;
    [SerializeReference] public IMovementData data;
    
}