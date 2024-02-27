using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            case PickableType.None:
                Debug.Log("Item type : " + item.TemplateType + ", no behavior linked to NONE");
                return;

            //StatModifier add a bonus to a specific stat
            case PickableType.StatModifier:
                stats.AddBonus(item.StatModifier);
                Debug.Log("Item Type : " + item.TemplateType + ", get Bonus/Malus to : " + item.StatModifier.Stat + ", current value : " + stats.GetUpdatedStat(item.StatModifier.Stat));
                break;

            //WeaponPickup equip a new weapon
            case PickableType.Weapon:
                //TODO: Spawn a PickupItem with EquippedWeapon in type
                slot.EquipWeapon(item.WeaponPickup.Weapon);
                Debug.Log("Item Type : " + item.TemplateType + ", equip new weapon : " + item.WeaponPickup.Weapon);
                break;

            //Consumable effect Switch
            case PickableType.consumable:
                switch (item.Consumable.Effect)
                {
                    case ConsumableEffect.None:
                        break;

                    case ConsumableEffect.Heal:
                        lifeSystem.HealUp(item.Consumable.Value);
                        Debug.Log("Item Type : " + item.TemplateType + ", name : " + item.Consumable.name + ", gain " + item.Consumable.Value + " LifePoints, for a total of " + lifeSystem.CurrentLifePoints + " on " + lifeSystem.MaxLifePoints);
                        break;

                    default:
                        break;

                }
                break;

            default:
                break;
        }

        //Destroy Pickup
        item.DelayedDestroy(despawnDelay);
    }
}
