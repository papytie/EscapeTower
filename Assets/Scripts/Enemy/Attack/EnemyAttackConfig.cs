using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackData", menuName = "GameData/EnemyAttackData", order = 1)]

[Serializable]
public class EnemyAttackConfig : ScriptableObject
{
    [Header("Attack Data"), Space]
    public EnemyAttackData attackData;

    [Header("Attack FX"), Space]
    public GameObject attackFXPrefab;

}
