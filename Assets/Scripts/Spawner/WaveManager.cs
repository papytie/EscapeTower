using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(WaveSpawner))]

public class WaveManager : MonoBehaviour
{
    enum BudgetIncreaseType
    {
        add = 0,
        mult = 1,
    }

    [Header("Enemies Layout")]
    [SerializeField] EnemyWaveLayout layout;

    [Header("Spawner Settings")]
    [SerializeField] int startBudget = 10;
    [SerializeField] BudgetIncreaseType increaseType = BudgetIncreaseType.add;
    [SerializeField] int addFactor = 5;
    [SerializeField] float multFactor = .1f;
    [SerializeField] int bossWaves = 5;

    int currentBudget;
    int waveCount;

    WaveSpawner spawner;

    Dictionary<GameObject, int> newWave = new();

    private void Awake()
    {
        spawner = GetComponent<WaveSpawner>();
    }

    private void Start()
    {
        
    }

    void GenerateNewWave(int budget)
    {
        newWave.Clear();

        int credit = budget;

        foreach(var enemy in layout.enemyList)
        {
            if(enemy.cost < credit)
            {
                int max = credit / enemy.cost;
                int number = Random.Range(0, max+1);
                credit -= number * enemy.cost; 
                newWave.Add(enemy.enemyPrefab, number);
            }
        }
    }
}
