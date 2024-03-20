using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackData", menuName = "GameData/EnemyAttackData", order = 1)]

[Serializable]
public class EnemyAttackData : ScriptableObject
{
    [Header("Attack Data"), Space]
    public AttackData attackData;

    [Header("Attack Type"), Space]
    public AttackType attackType;

    [Header("Attack FX"), Space]
    [SerializeReference] public IAttackFX attackFX;

}
