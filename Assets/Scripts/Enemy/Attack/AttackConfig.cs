using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackConfig
{
    public AttackType type = AttackType.Melee;
    [SerializeReference] public IAttackData attackData;
    public GameObject attackFXPrefab;
}
