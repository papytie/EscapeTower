using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickup", menuName = "Scripts/Items/ScriptableObjects/StatModifierTemplate", order = 1)]
public class StatModifierTemplate : ScriptableObject, IPickup
{
    public StatConcerned Stat => stat;
    public float ModifValue => modifValue;

    [SerializeField] string pickupName;
    [SerializeField] float modifValue;
    [SerializeField] PickableType type;
    [SerializeField] StatConcerned stat;
    [SerializeField] Sprite itemSprite;

    public PickableType Type { get => type; }
    public Sprite Sprite { get => itemSprite; }

}

public enum StatConcerned
{
    None = 0,
    MoveSpeed = 1,
    Damage = 2,
}
