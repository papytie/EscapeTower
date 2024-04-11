using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    
   
}
