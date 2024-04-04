using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "EscapeTower/Items/Create Weapon", order = 1)]
public class WeaponPickupTemplate : ScriptableObject, IPickup
{
    public WeaponController Weapon => weapon;

    [Header("Name")]
    [SerializeField] string pickupName;

    [Header("Settings")]
    [SerializeField] WeaponController weapon;
    [SerializeField] Sprite itemSprite;

    public PickableType Type => PickableType.Weapon;
    public Sprite Sprite { get => itemSprite; }
}

