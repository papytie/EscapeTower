using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class PickupItem : MonoBehaviour
{
    public bool IsDespawning => isDespawning;
    public PickableType TemplateType => templateType;
    public StatModifierTemplate StatModifier => statModifierTemplate;
    public ConsumableTemplate Consumable => consumableTemplate;
    public WeaponPickupTemplate WeaponPickup => weaponPickupTemplate;

    [SerializeField] PickableType templateType;
    [SerializeField] StatModifierTemplate statModifierTemplate;
    [SerializeField] ConsumableTemplate consumableTemplate;
    [SerializeField] WeaponPickupTemplate weaponPickupTemplate;

    IPickup template;
    SpriteRenderer spriteRenderer;

    float despawnDelay = 1;
    float despawnTime = 0;
    bool isDespawning = false;

    void Init(IPickup templateToApply)
    {
        template = templateToApply;
        templateType = template.Type;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = template.Sprite;
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
            case PickableType.consumable:
                template = consumableTemplate;
                break;
            default:
                break;
        }

        Init(template);
    }

    private void Update()
    {
        if (isDespawning && TimeUtils.CustomTimer(ref despawnTime, despawnDelay))
            Destroy(gameObject);

    }

    public void DelayedDestroy(float despawnDelayRef)
    {
        despawnDelay = despawnDelayRef;
        isDespawning = true;
    }

    void Animation()
    {
        //TODO: trigger animation when pickup
    }
}



