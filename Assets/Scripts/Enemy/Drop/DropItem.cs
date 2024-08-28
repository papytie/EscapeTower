using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    DropItemConfig config;
    EnemyDropComponent drop;
    float spawnTime = 0;

    private void OnEnable()
    {
        //Increment public static Count
    }

    public void Init(DropItemConfig dropConfig, EnemyDropComponent component)
    {
        config = dropConfig;
        drop = component;
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= spawnTime + config.lifespan && !config.isEternal)
        {
            drop.DecrementCount(config);
            Destroy(gameObject);
        }
    }

    public float GetDamage()
    {
        return config.damage;
    }

    private void OnDisable()
    {
        //Decrement public static Count
    }
}
