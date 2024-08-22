using System;
using UnityEngine;

[Serializable]
public class EnemyConfig
{
    public EnemyController enemyPrefab;
    public int cost;
    public int value;
    public EnemyType type = EnemyType.Standard;
}