using UnityEngine;

public class EnemyLifeSystemComponent : MonoBehaviour, ILifeSystem
{
    public bool IsDead => isDead;
    public float CurrentLifePoints => currentLifePoints;
    public float MaxLifePoints => controller.Stats.MaxLifePoints;

    [Header("Life Settings")]
    [SerializeField] float despawnDuration = 3f;

    bool isDead = false;
    float despawnEndTime = 0;
    float currentLifePoints = 1;

    EnemyController controller;
    Collider2D enemyCollider;

    public void InitRef(EnemyController ctrlRef)
    {
        controller = ctrlRef;
    }

    private void Awake()
    {
        enemyCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isDead && Time.time >= despawnEndTime)
        {
            controller.LootSystem.RollLoot();
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damageValue, Vector2 normal)
    {
        if (isDead) return;

        currentLifePoints -= damageValue;
        if (currentLifePoints <= 0)
        {
            currentLifePoints = 0;
            SetDespawnTimer(despawnDuration);
            enemyCollider.enabled = false;
            controller.AnimationParam.ActivateDieTrigger();
            return;
        }
        controller.Bump.BumpedAwayActivation(-normal, damageValue);
        controller.AnimationParam.ActivateTakeDamageTrigger();
    }

    public void HealUp(float healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, controller.Stats.MaxLifePoints);
    }

    void SetDespawnTimer(float duration)
    {
        despawnEndTime = Time.time + duration;
        isDead = true;
    }

}
