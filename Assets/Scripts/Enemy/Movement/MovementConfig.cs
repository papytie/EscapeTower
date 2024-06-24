using System;
using UnityEngine;

[Serializable]
public class MovementConfig
{
    public MovementType MoveType => type;

    public MovementConfig(MovementType moveType)
    {
        type = moveType;
        name = moveType.ToString();
    }

    public string name;
    protected MovementType type;
    [SerializeReference] public IMovementData data;
}