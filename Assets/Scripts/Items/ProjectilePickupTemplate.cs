using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "EscapeTower/Items/Create Projectile", order = 1)]
public class ProjectilePickupTemplate : ScriptableObject, IPickup
{
    public WeaponProjectile Projectile => projectile;

    [Header("Name")]
    [SerializeField] string pickupName;

    [Header("Settings")]
    [SerializeField] WeaponProjectile projectile;
    [SerializeField] Sprite itemSprite;

    public PickableType Type => PickableType.Projectile;
    public Sprite Sprite { get => itemSprite; }
}

