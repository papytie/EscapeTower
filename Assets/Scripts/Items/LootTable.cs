using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;

[CreateAssetMenu(fileName = "LootTable", menuName = "GameData/NewLootTable", order = 1)]

[Serializable]
public class LootItem
{
    public GameObject pickup;
    public int lootWeight;
}

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
                if(!lootWeightDict.ContainsKey(i))
                {
                    lootWeightDict.Add(weightPoolCount, item.pickup);
                    weightPoolCount += 1;
                }
            }
        }
    }

    public GameObject PickRandomLoot()
    {
        int randomValue = UnityEngine.Random.Range(0, lootWeightDict.Count);
        return lootWeightDict[randomValue];

    }

}
