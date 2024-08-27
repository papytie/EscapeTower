using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    EnemyDropComponent dropComponent;
    DropItemConfig config;
    float spawnTime = 0;

    public void Init(EnemyDropComponent drop, DropItemConfig dropConfig)
    {
        dropComponent = drop;
        config = dropConfig;
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= spawnTime + config.lifespan && !config.isEternal)
        {
            dropComponent.DecrementCount(config);
            Destroy(gameObject);
        }
    }

    public float GetDamage()
    {
        return config.damage;
    }
}
