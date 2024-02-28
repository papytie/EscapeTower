using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "EscapeTower/Items/Create Consumable", order = 1)]
public class ConsumableTemplate : ScriptableObject, IPickup
{
    public float Value => value;
    public ConsumableEffect Effect => effect;

    [Header("Type")]
    [SerializeField] PickableType type;

    [Header("Name")]
    [SerializeField] string pickupName;

    [Header("Settings")]
    [SerializeField] ConsumableEffect effect;
    [SerializeField] float value;
    [SerializeField] Sprite itemSprite;

    public PickableType Type { get => type; }
    public Sprite Sprite { get => itemSprite; }
}

public enum ConsumableEffect
{
    Heal = 0
}

