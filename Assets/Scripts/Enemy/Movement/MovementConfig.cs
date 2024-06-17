using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovementConfig
{
    //Constructor for config list initialisation
    public MovementConfig(MovementType moveType)
    {
        type = moveType;
    }

    public MovementType type = MovementType.Wait;
    [SerializeReference] public IMovementData data;
}