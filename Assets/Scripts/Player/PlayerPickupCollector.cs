using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerPickupCollector : MonoBehaviour
{
    [SerializeField] float despawnDelay = 1;

    PlayerStats stats;
    PlayerWeaponSlot slot;
    PlayerLifeSystem lifeSystem;

    public void InitRef(PlayerStats statsRef, PlayerWeaponSlot slotRef, PlayerLifeSystem lifeSystemRef)
    {
        stats = statsRef;
        slot = slotRef;
        lifeSystem = lifeSystemRef;
    }

    public void PickUpSorting(PickupItem item)
    {
        switch (item.TemplateType)
        {
            //StatModifier add a bonus to a specific stat
            case PickableType.StatModifier:
                stats.AddBonus(item.StatModifier);
                Debug.Log("Item Type : " + item.TemplateType + ", get Bonus/Malus to : " + item.StatModifier.MainStat + ", current value : " + stats.GetModifiedMainStat(item.StatModifier.MainStat));
                break;

            //WeaponPickup equip a new weapon
            case PickableType.Weapon:
                //TODO: Spawn a PickupItem with EquippedWeapon in type
                slot.EquipWeapon(item.WeaponPickup.Weapon);
                Debug.Log("Item Type : " + item.TemplateType + ", equip new weapon : " + item.WeaponPickup.Weapon);
                break;

            //Consumable effect Switch
            case PickableType.Consumable:
                UseConsumable(item);
                Debug.Log("Item Type : " + item.TemplateType + ", name : " + item.Consumable.name + ", gain " + item.Consumable.Value + " LifePoints, for a total of " + lifeSystem.CurrentLifePoints + " on " + lifeSystem.MaxLifePoints);
                break;

            default:
                break;
        }

        //Destroy Pickup
        item.Pick();
    }

    void UseConsumable(PickupItem item)
    {
        switch (item.Consumable.Effect)
        {
            case ConsumableEffect.Heal:
                lifeSystem.HealUp(item.Consumable.Value);
                break;
            default:
                break;

        }
    }
}
