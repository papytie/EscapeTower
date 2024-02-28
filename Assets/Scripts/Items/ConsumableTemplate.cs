using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickup", menuName = "Scripts/Items/ScriptableObjects/ConsumableTemplate", order = 1)]
public class ConsumableTemplate : ScriptableObject, IPickup
{
    public float Value => value;
    public ConsumableEffect Effect => effect;

    [SerializeField] string pickupName;
    [SerializeField] PickableType type;
    [SerializeField] ConsumableEffect effect;
    [SerializeField] float value;
    [SerializeField] Sprite itemSprite;

    public PickableType Type { get => type; }
    public Sprite Sprite { get => itemSprite; }
}

public enum ConsumableEffect
{
    None = 0,
    Heal = 1,
}

