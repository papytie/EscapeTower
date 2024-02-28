using UnityEngine;

[CreateAssetMenu(fileName = "Pickup", menuName = "Scripts/Items/ScriptableObjects/WeaponPickupTemplate", order = 1)]
public class WeaponPickupTemplate : ScriptableObject, IPickup
{
    public PlayerWeapon Weapon => weapon;

    [Header("Type")]
    [SerializeField] PickableType type;

    [Header("Name")]
    [SerializeField] string pickupName;

    [Header("Settings")]
    [SerializeField] PlayerWeapon weapon;
    [SerializeField] Sprite itemSprite;

    public PickableType Type { get => type; }
    public Sprite Sprite { get => itemSprite; }
}

