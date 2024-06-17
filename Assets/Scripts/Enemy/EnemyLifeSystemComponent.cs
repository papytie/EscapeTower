using UnityEngine;

public class EnemyLifeSystemComponent : MonoBehaviour, ILifeSystem
{
    public bool IsDead => isDead;
    public float CurrentLifePoints => currentLifePoints;
    public float MaxLifePoints => stats.MaxLifePoints;

    [Header("Life Settings")]
    [SerializeField] float despawnDuration = 3f;

    bool isDead = false;
    float despawnEndTime = 0;
    float currentLifePoints = 1;

    Animator animator;
    EnemyStatsComponent stats;
    EnemyLootSystem enemyLoot;
    Collider2D enemyCollider;
    BumpComponent bump;

    public void InitRef(Animator animatorRef, EnemyLootSystem lootSystem, BumpComponent bumpRef, EnemyStatsComponent enemyStats)
    {
        animator = animatorRef;
        stats = enemyStats;
        currentLifePoints = stats.MaxLifePoints;
        enemyLoot = lootSystem;
        bump = bumpRef;
    }

    private void Start()
    {
        enemyCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isDead && Time.time >= despawnEndTime)
        {
            enemyLoot.RollLoot();
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
            animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.die);
            return;
        }
        bump.BumpedAwayActivation(-normal, damageValue);
        animator.SetTrigger(SRAnimators.EnemyBaseAnimator.Parameters.takeDamage);
    }

    public void HealUp(float healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, stats.MaxLifePoints);
    }

    void SetDespawnTimer(float duration)
    {
        despawnEndTime = Time.time + duration;
        isDead = true;
    }

}
