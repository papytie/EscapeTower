using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaveSpawner))]

public class WaveManagement : MonoBehaviour
{
    enum BudgetIncreaseType
    {
        add = 0,
        mult = 1,
    }

    [Header("Enemies Layout")]
    [SerializeField] EnemyWaveLayout layout;

    [Header("Wave Settings")]
    [SerializeField] int startBudget = 10;
    [SerializeField] int minWaveSize = 4;
    [SerializeField] BudgetIncreaseType increaseType = BudgetIncreaseType.add;
    [SerializeField] int addFactor = 5;
    [SerializeField] float multFactor = .1f;
    [SerializeField] int bossWaves = 5;
       
    public event Action OnWaveClear;

    Dictionary<EnemyController, int> newWaveLayout = new();
    List<EnemyConfig> enemyLayoutCopy;
    WaveSpawner waveSpawner;

    int waveCount = 0;
    int currentBudget = 0;

    private void Awake()
    {
        waveSpawner = GetComponent<WaveSpawner>();
        enemyLayoutCopy = layout.GetListCopy();
    }

    private void Start()
    {
        OnWaveClear += () =>
        {
            waveCount++;
            currentBudget = UpdateBudget(startBudget, waveCount);
            //TODO : Update scaling factor
            ShuffleEnemyLayout();
            newWaveLayout = GenerateNewWaveLayout(currentBudget);
            waveSpawner.SpawnWave(newWaveLayout, 1);
        };
    }

    private void Update()
    {
        if(EnemyManager.Instance.EnemyList.Count == 0)
            OnWaveClear.Invoke();
    }

    void ShuffleEnemyLayout()
    {
        //Shuffle Enemy List for a better Randomization
        var count = enemyLayoutCopy.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var randomIndex = UnityEngine.Random.Range(i, count);
            var tempIndex = enemyLayoutCopy[i];
            enemyLayoutCopy[i] = enemyLayoutCopy[randomIndex];
            enemyLayoutCopy[randomIndex] = tempIndex;
        }
    }

    int UpdateBudget(int start, int waveCount)
    {
        return (increaseType) switch
        {
            BudgetIncreaseType.mult => Mathf.RoundToInt(start * (1 + (multFactor * waveCount))),
            _ => start + addFactor * waveCount,
        };
        
    }

    Dictionary<EnemyController, int> GenerateNewWaveLayout(int budget)
    {
        int credit = budget;

        Dictionary<EnemyController, int> newLayout = new();

        //Generate random enemies in list for each entry
        foreach(var enemy in enemyLayoutCopy)
        {
            if(enemy.cost <= budget / minWaveSize)
            {
                int max = credit / enemy.cost;
                int number = UnityEngine.Random.Range(0, max);

                if(number > 0)
                {
                    credit -= number * enemy.cost;
                    newLayout.Add(enemy.enemyPrefab, number);
                }
            }
        }

        //Spend maximum credits possible
        if(credit > 0) 
        {
            foreach(var enemy in enemyLayoutCopy)
            {
                if(enemy.cost <= credit) 
                {
                    int max = credit / enemy.cost;
                    credit -= max * enemy.cost;

                    if(newLayout.ContainsKey(enemy.enemyPrefab))
                        newLayout[enemy.enemyPrefab] += max;
                    else newLayout.Add(enemy.enemyPrefab, max);
                }
            }
        }

        return newLayout;
    }
}
