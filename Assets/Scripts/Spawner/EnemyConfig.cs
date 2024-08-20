using System;
using UnityEngine;

[Serializable]
public class EnemyConfig
{
    public GameObject enemyPrefab;
    public int cost;
    public int value;
    public EnemyType type = EnemyType.Standard;
}