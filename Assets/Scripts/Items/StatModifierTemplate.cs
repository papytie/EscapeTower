using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickup", menuName = "Scripts/Items/ScriptableObjects/StatModifierTemplate", order = 1)]
public class StatModifierTemplate : ScriptableObject, IPickup
{
    public MainStat MainStat => mainStat;
    public float ModifValue => modifValue;
    public ValueType ValueType => valueType;
    public CalculType Calcul => calculType;

    [Header("Type")]
    [SerializeField] PickableType type;

    [Header("Name")]
    [SerializeField] string pickupName;

    [Header("Settings")]
    [SerializeField] MainStat mainStat;
    [SerializeField] float modifValue;
    [SerializeField] ValueType valueType;
    [SerializeField] CalculType calculType;
    [SerializeField] Sprite itemSprite;

    public PickableType Type { get => type; }
    public Sprite Sprite { get => itemSprite; }

}
