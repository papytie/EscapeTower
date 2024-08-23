using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Span zone Settings"), Space]
    [SerializeField] PlayerController player;
    [SerializeField] Vector2 zoneSize = new(10, 10);
    [SerializeField] float deadZoneRadius = 1;

    public void SpawnWave(Dictionary<EnemyController, int> nextWave, float scalingFactor)
    {
        foreach(var enemy in nextWave) 
        {
            for (int i = 0; i < enemy.Value; i++)
            {
                EnemyController newEnemy = Instantiate(enemy.Key, RandomPos(), Quaternion.identity);
                newEnemy.Stats.SetScalingFactorTo(scalingFactor);
                Debug.Log("Spawn" + enemy.Key);
            }
        }
    }

    Vector2 RandomPos()
    {
        Vector2 randPos = new(Random.Range(-zoneSize.x / 2, zoneSize.x / 2), Random.Range(-zoneSize.y / 2, zoneSize.y / 2));
        if (Vector2.Distance(randPos, player.transform.position) < deadZoneRadius) return RandomPos();
        return randPos;
    }
}
