using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SurvivorSpawner : MonoBehaviour
{
    [Header("Zone Settings"), Space]
    [SerializeField] PlayerController player;
    [SerializeField] Vector2 zoneSize = new(10,10);
    [SerializeField] float deadZoneRadius = 1;

    [Header("Waves Settings"), Space]
    [SerializeField] int startSpawnPoints = 10;
    [SerializeField] int wavesNumber = 10;
    [SerializeField] bool unlimitedWaves = false;
    [SerializeField] bool waveGrowth = true;
    [SerializeField] float growthFactor = 1.2f;

    [Header("Enemy List"), Space]
    [SerializeField] List<EnemyToSpawn> enemyToSpawnList = new();

    [Header("Debug"), Space]
    [SerializeField] bool showDebug = true;
    [SerializeField] Color zoneColor = Color.green;
    [SerializeField] Color deadzoneColor = Color.red;
    [SerializeField] Color randomPointColor = Color.blue;

    List<EnemyController> currentEnemyList = new();
    List<EnemyToSpawn> tier1List = new();
    List<EnemyToSpawn> tier2List = new();
    List<EnemyToSpawn> tier3List = new();

    int currentSpawnPoints = 10;
    int waveCount = 0;
    Vector2 currentSpawnPos = Vector2.zero;
    float endTime = 0;

    void Start()
    {
        currentSpawnPoints = startSpawnPoints;
        GenerateTierList();
    }

    void Update()
    {
        if(Time.time >= endTime) 
        {
            endTime = Time.time + 5;
            Debug.Log("End Time is :" + endTime);
            if(waveCount > wavesNumber && !unlimitedWaves)
            {
                Debug.Log("Enemies are defeated!");
                return;
            }            
            CheckEnemyList();
        }
    }

    void CheckEnemyList()
    {
        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            if (currentEnemyList[i] == null)
                currentEnemyList.Remove(currentEnemyList[i]);
            
        }
        if (currentEnemyList.Count == 0)
        {
            Debug.LogWarning("EnemyList is empty");
            currentEnemyList.Clear();
            ResetSpawnPoints();
            SpawnWave();
        }
    }

    void ResetSpawnPoints()
    {
        currentSpawnPoints = waveGrowth ? (int)Mathf.Round(startSpawnPoints * (1 + (growthFactor * waveCount))) : startSpawnPoints;
        waveCount++;
        Debug.Log("CurrentSpawnPoints :" + currentSpawnPoints);
    }

    Vector2 RandomPoint()
    {
        Vector2 randomPos = new(UnityEngine.Random.Range(-zoneSize.x / 2, zoneSize.x / 2), UnityEngine.Random.Range(-zoneSize.y / 2, zoneSize.y / 2));
        if (Vector2.Distance(randomPos, player.transform.position) < deadZoneRadius) return RandomPoint();
        return randomPos;
    }

    void SpawnWave()
    {
        if(tier3List.Count > 0)
        {
            SpawnEnemies(UnityEngine.Random.Range(0, currentSpawnPoints/5), tier3List);
        }
        
        if(tier2List.Count > 0)
        {
            SpawnEnemies(UnityEngine.Random.Range(0, currentSpawnPoints/3), tier2List);
        }
        
        if(tier1List.Count > 0)
        {
            SpawnEnemies(currentSpawnPoints, tier1List);
        }
    }

    void SpawnEnemies(int instances, List<EnemyToSpawn> list)
    {
        for (int i = 0; i < instances; i++)
        {
            EnemyController enemy = Instantiate(PickRandomEnemyInList(list), RandomPoint(), Quaternion.identity);
            currentEnemyList.Add(enemy);
        }
    }

    EnemyController PickRandomEnemyInList(List<EnemyToSpawn> list)
    {
        int rand = UnityEngine.Random.Range(0, list.Count);
        currentSpawnPoints -= list[rand].enemyCost;
        return list[rand].enemyPrefab;
    }

    void GenerateTierList()
    {
        foreach (EnemyToSpawn item in enemyToSpawnList)
        {
            switch(item.tier)
            {
                case EnemyTier.Tier1:
                    if(!tier1List.Contains(item))
                        tier1List.Add(item);
                    break;

                case EnemyTier.Tier2:
                    if(!tier2List.Contains(item))
                    tier2List.Add(item);
                    break;

                case EnemyTier.Tier3:
                    if(!tier3List.Contains(item))
                    tier3List.Add(item);
                    break;

            }
        }
    }

    private void OnDrawGizmos()
    {
        if(showDebug)
        {
            Gizmos.color = zoneColor;
            Gizmos.DrawCube(transform.position, zoneSize);
            Gizmos.color = deadzoneColor;
            Gizmos.DrawSphere(player.transform.position, deadZoneRadius);
            Gizmos.color = randomPointColor;
            Gizmos.DrawWireSphere(currentSpawnPos, .1f);
            Gizmos.color = Color.white;
        }
    }
}

[Serializable]
public class EnemyToSpawn
{
    public EnemyType type = EnemyType.Standard;
    public EnemyTier tier = EnemyTier.Tier1;
    public int enemyCost = 1;
    public EnemyController enemyPrefab;
}

public enum EnemyTier
{
    Tier1 = 0,
    Tier2 = 1,
    Tier3 = 2,
}

public enum EnemyType
{
    Standard = 0,
    Boss = 1,
}
