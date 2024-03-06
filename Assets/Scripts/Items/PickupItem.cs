using DG.Tweening;
using UnityEngine;
using UnityEngine.U2D;

public class PickupItem : MonoBehaviour
{
    public bool IsDespawning { get; private set; }
    public PickableType TemplateType => templateType;
    public StatModifierTemplate StatModifier => statModifierTemplate;
    public ConsumableTemplate Consumable => consumableTemplate;
    public WeaponPickupTemplate WeaponPickup => weaponPickupTemplate;

    [SerializeField] PickableType templateType;
    [SerializeField] StatModifierTemplate statModifierTemplate;
    [SerializeField] ConsumableTemplate consumableTemplate;
    [SerializeField] WeaponPickupTemplate weaponPickupTemplate;

    [Header("Visual")]
    [SerializeField] SpriteRenderer borderSprite;
    [SerializeField] SpriteRenderer contentSprite;
    [SerializeField] float scaleAnim = 2f;
    [SerializeField] float scaleAnimDuration = 1f;
    [SerializeField] float fadeAnimDuration = 0.5f;

    IPickup template;

    void Init(IPickup templateToApply)
    {
        template = templateToApply;
        templateType = template.Type;
        contentSprite.sprite = template.Sprite;
        borderSprite.color = GetBorderColor();
    }

    private void Start()
    {
        switch (templateType) 
        {
            case PickableType.Weapon:
                template = weaponPickupTemplate;
                break;
            case PickableType.StatModifier: 
                template = statModifierTemplate;
                break;
            case PickableType.Consumable:
                template = consumableTemplate;
                break;
            default:
                break;
        }

        Init(template);
    }

    private Color GetBorderColor() {
        return template.Type switch {
            PickableType.Weapon => Game.GameSettings.Pickup.weaponColor,
            PickableType.Consumable => Game.GameSettings.Pickup.consumableColor,
            PickableType.StatModifier => Game.GameSettings.Pickup.bonusColor,
            _ => Color.white
        };
    }

    public void Pick()
    {
        IsDespawning = true;
        borderSprite.sortingLayerName = SRSortingLayers.UI;
        contentSprite.sortingLayerName = SRSortingLayers.UI;
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(scaleAnim, scaleAnimDuration).SetEase(Ease.OutQuad))
            .Join(borderSprite.DOFade(0f, fadeAnimDuration).SetEase(Ease.OutQuad))
            .Join(contentSprite.DOFade(0f, fadeAnimDuration).SetEase(Ease.OutQuad))
            .OnComplete(() => Destroy(gameObject));
    }
}



