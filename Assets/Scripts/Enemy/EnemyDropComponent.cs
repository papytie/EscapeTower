using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemyDropComponent : MonoBehaviour
{
    EnemyController enemyController;
    Dictionary<DropItemConfig, int> itemDroppedCount = new();
    Dictionary<DropItemConfig, Vector2> itemDroppedLastPos = new();

    public void Init(EnemyController controller)
    {
        enemyController = controller;
    }

    public void DropItem(DropItemConfig config)
    {
        Quaternion currentRotation = Quaternion.LookRotation(Vector3.forward, enemyController.MainBehaviour.FSM.CurrentState.Action.Direction);
        Vector3 dropPosition = transform.position + (currentRotation * config.dropPositionOffset);

        if (IsValidPosition(config, dropPosition)) 
        {
            DropItem item = Instantiate(config.item, dropPosition, Quaternion.identity);
            item.Init(this, config);
            IncrementCount(config);
            RecordPosition(config, dropPosition);
        }
    }

    void IncrementCount(DropItemConfig config)
    {
        if (!itemDroppedCount.ContainsKey(config))
            itemDroppedCount.Add(config, 1);

        itemDroppedCount[config]++;
    }

    void RecordPosition(DropItemConfig config, Vector2 position)
    {
        if (!itemDroppedLastPos.ContainsKey(config))
            itemDroppedLastPos.Add(config, position);

        itemDroppedLastPos[config] = position;
    }

    public void DecrementCount(DropItemConfig config)
    {
        if (itemDroppedCount.ContainsKey(config))
        {
            itemDroppedCount[config]--;

            if (itemDroppedCount[config] <= 0)
                itemDroppedCount.Remove(config);
        }
    }

    bool IsValidPosition(DropItemConfig config, Vector2 position)
    {
        if (itemDroppedLastPos.ContainsKey(config))
            if (Vector2.Distance(position, itemDroppedLastPos[config]) < config.minDistBetweenItems)
                return false;

        return true;
    }
/*    
 *  private void OnValidate()
    {
        foreach (var item in dropList)
        {
            Type type = item.type switch
            {
                DropType.Timer => typeof(DropTimerData),
                DropType.Damage => typeof(DropDamageData),
                DropType.Die => typeof(DropDieData),
                _ => null,
            };

            if ((item.data == null && type != null) || (item.data != null && item.data.GetType() != type))
            {
                item.data = DropItemDataFactory.Create(item.type);
            }
        }
    }
*/
}
