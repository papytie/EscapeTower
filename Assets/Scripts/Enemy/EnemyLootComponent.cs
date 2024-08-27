using System.Collections.Generic;
using UnityEngine;

public class EnemyLootComponent : MonoBehaviour
{
    public LootTable LootTable => lootTable;

    [Header("Loot Settings")]
    [SerializeField] LootTable lootTable;
    [SerializeField] float lootChance = 20;

    public void RollLoot()
    {
        float rollValue = Random.value;
        if (rollValue > 0f && rollValue <= lootChance / 100)
        {
            //Debug.Log("loot roll : " + rollValue + "You got some juicy loot !");
            Instantiate(lootTable.PickRandomLoot(), transform.position, Quaternion.identity);
        }
        //else Debug.Log("loot roll : " + rollValue + "Sorry, no loot for you this time :(");
    }
}
