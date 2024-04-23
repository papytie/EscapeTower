using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootItem
{
    public GameObject pickup;
    public int lootWeight;
}

[CreateAssetMenu(fileName = "LootTable", menuName = "GameData/NewLootTable", order = 1)]

[Serializable]
public class LootTable : ScriptableObject
{
    public List<LootItem> LootList = new();
    public Dictionary<int, GameObject> lootWeightDict = new();

    private void OnEnable()
    {
        InitLootTable();
    }

    private void OnValidate()
    {
        InitLootTable();
    }

    void InitLootTable()
    {
        lootWeightDict.Clear();
        int weightPoolCount = 0;
        foreach (LootItem item in LootList)
        {
            for (int i = 0; i < item.lootWeight; i++)
            {
                lootWeightDict.Add(weightPoolCount, item.pickup);
                weightPoolCount += 1;
            }
        }
        //Debug.Log("Weight Dictionnary :" + lootWeightDict.Count);
    }

    public GameObject PickRandomLoot()
    {
        int randomValue = UnityEngine.Random.Range(0, lootWeightDict.Count);
        //Debug.Log("Loot Table roll :" + randomValue);
        return lootWeightDict[randomValue];

    }

}
