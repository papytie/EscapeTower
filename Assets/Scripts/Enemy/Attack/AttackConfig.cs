using System;
using UnityEngine;

[Serializable]
public class AttackConfig
{
    public AttackType AttackType => type;

    public AttackConfig(AttackType attackType)
    {
        type = attackType;
        name = attackType.ToString();
    }

    public string name;
    protected AttackType type = AttackType.Melee;
    [SerializeReference] public IAttackData data;
    public GameObject attackFXPrefab;
}
