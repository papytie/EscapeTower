using UnityEngine;

[CreateAssetMenu(fileName = "Bonus", menuName = "EscapeTower/Items/Create Bonus", order = 1)]
public class StatModifierTemplate : ScriptableObject, IPickup
{
    public MainStat MainStat => mainStat;
    public float ModifValue => modifValue;
    public ValueType ValueType => valueType;
    public CalculType Calcul => calculType;

    [Header("Name")]
    [SerializeField] string pickupName;

    [Header("Settings")]
    [SerializeField] MainStat mainStat;
    [SerializeField] float modifValue;
    [SerializeField] ValueType valueType;
    [SerializeField] CalculType calculType;
    [SerializeField] Sprite itemSprite;

    public PickableType Type => PickableType.StatModifier;
    public Sprite Sprite { get => itemSprite; }

}
