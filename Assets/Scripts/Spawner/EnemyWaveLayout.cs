using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyWaveLayout", menuName = "GameData/EnemyWaveLayout", order = 1)]

public class EnemyWaveLayout : ScriptableObject
{
    public List<EnemyConfig> enemyList = new();
}