using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "EscapeTower/Items/Create Projectile", order = 1)]
public class ProjectilePickupTemplate : ScriptableObject, IPickup
{
    public ProjectileController Projectile => projectile;

    [Header("Name")]
    [SerializeField] string pickupName;

    [Header("Settings")]
    [SerializeField] ProjectileController projectile;
    [SerializeField] Sprite itemSprite;

    public PickableType Type => PickableType.Projectile;
    public Sprite Sprite { get => itemSprite; }
}

