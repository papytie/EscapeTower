using System;
using UnityEngine;

[Serializable]
public class BehaviourConfig
{
    public BehaviourType behaviourType;
    [SerializeReference] public IBehaviourData behaviourData;
    public IBehaviour behaviour;
}
