using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour
{
    PlayerController controller;

    public void InitRef(PlayerController playerController)
    {
        controller = playerController;
    }

    public void PickUpSorting(PickupItem item)
    {
        switch (item.TemplateType)
        {
            //StatModifier add a bonus to a specific stat
            case PickableType.StatModifier:
                controller.Stats.AddBonus(item.StatModifier);
                Debug.Log("Item Type : " + item.TemplateType + ", get Bonus/Malus to : " + item.StatModifier.MainStat + ", current value : " + controller.Stats.GetModifiedMainStat(item.StatModifier.MainStat));
                break;

            //WeaponPickup equip a new weapon
            case PickableType.Weapon:
                //TODO: Spawn a PickupItem with EquippedWeapon in type
                controller.WeaponSlot.EquipWeapon(item.WeaponPickup.Weapon);
                Debug.Log("Item Type : " + item.TemplateType + ", equip new weapon : " + item.WeaponPickup.Weapon);
                break;

            //Consumable effect Switch
            case PickableType.Consumable:
                UseConsumable(item);
                Debug.Log("Item Type : " + item.TemplateType + ", name : " + item.Consumable.name + ", gain " + item.Consumable.Value + " LifePoints, for a total of " + controller.LifeSystem.CurrentLifePoints + " on " + controller.LifeSystem.MaxLifePoints);
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
                controller.LifeSystem.HealUp(item.Consumable.Value);
                break;
            default:
                break;

        }
    }
}
