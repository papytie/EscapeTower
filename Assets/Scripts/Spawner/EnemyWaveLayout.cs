using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyWaveLayout", menuName = "GameData/EnemyWaveLayout", order = 1)]

public class EnemyWaveLayout : ScriptableObject
{
    [SerializeField] List<EnemyConfig> enemyLayout = new();

    public List<EnemyConfig> GetListCopy()
    {
        List<EnemyConfig> list = new();
        foreach (EnemyConfig config in enemyLayout)
            list.Add(config);
        return list;
    }
}