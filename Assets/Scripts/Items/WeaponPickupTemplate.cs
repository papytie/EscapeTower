using UnityEngine;

[CreateAssetMenu(fileName = "Pickup", menuName = "Scripts/Items/ScriptableObjects/WeaponPickupTemplate", order = 1)]
public class WeaponPickupTemplate : ScriptableObject, IPickup
{
    public PlayerWeapon Weapon => weapon;

    [SerializeField] string pickupName;
    [SerializeField] PlayerWeapon weapon;
    [SerializeField] PickableType type;
    [SerializeField] Sprite itemSprite;

    public PickableType Type { get => type; }
    public Sprite Sprite { get => itemSprite; }
}

