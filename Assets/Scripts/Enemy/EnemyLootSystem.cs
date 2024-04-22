using System.Collections.Generic;
using UnityEngine;

public class EnemyLootSystem : MonoBehaviour
{
    public LootTable Loot => loot;

    [Header("Loot Settings")]
    [SerializeField] LootTable loot;
    [SerializeField] float lootChance = 20;

    public void RollLoot()
    {
        float rollValue = UnityEngine.Random.value;
        if (rollValue > 0f && rollValue <= lootChance / 100)
        {
            //Debug.Log("loot roll : " + rollValue + "You got some juicy loot !");
            Instantiate(loot.PickRandomLoot(), transform.position, Quaternion.identity);
        }
        //else Debug.Log("loot roll : " + rollValue + "Sorry, no loot for you this time :(");
    }
}
