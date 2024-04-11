using System.Collections.Generic;
using UnityEngine;

public class EnemyLootSystem : MonoBehaviour
{
    [Header("Loot Settings")]
    [SerializeField] LootTable LootTable;
    [SerializeField] float lootChance = 20;

    Dictionary<int, GameObject> lootWeightDict = new();

    public void InitLootTable()
    {
        int weightPoolCount = 0;
        foreach (LootItem item in LootTable.LootList)
        {
            for (int i = 0; i < item.lootWeight; i++)
            {
                lootWeightDict.Add(weightPoolCount, item.pickup);
                weightPoolCount += 1;
            }
        }
    }

    public void RollLoot()
    {
        float rollValue = Random.value;
        if (rollValue > 0f && rollValue <= lootChance/100)
        {
            Debug.Log("loot roll : " + rollValue + "You got some juicy loot !");
            SpawnLoot();
        }
        else Debug.Log("loot roll : " + rollValue + "Sorry, no loot for you this time :(");
    }
    
    void SpawnLoot()
    {
        int randomValue = Random.Range(0, lootWeightDict.Count);
        Debug.Log("randomValue is : " + randomValue + " loot is " + lootWeightDict[randomValue].name);
        Instantiate(lootWeightDict[randomValue], transform.position, Quaternion.identity);
    }

}
