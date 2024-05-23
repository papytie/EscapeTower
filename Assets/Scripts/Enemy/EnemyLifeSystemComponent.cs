using UnityEngine;

public class EnemyLifeSystemComponent : MonoBehaviour, ILifeSystem
{
    public bool IsDead => isDead;
    public float CurrentLifePoints => currentLifePoints;
    public float MaxLifePoints => maxLifePoints;
    public float CollisionDamage => collisionDamage;

    [Header("Life Settings")]
    [SerializeField] float currentLifePoints = 10;
    [SerializeField] float maxLifePoints = 20;
    [SerializeField] float despawnDuration = 3f;
    [SerializeField] float collisionDamage = 1f;

    bool isDead = false;
    float despawnEndTime;

    Animator animator;
    EnemyLootSystem enemyLoot;
    Collider2D enemyCollider;
    BumpComponent bump;

    public void InitRef(Animator animatorRef, EnemyLootSystem lootSystem, BumpComponent bumpRef)
    {
        animator = animatorRef;
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

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

    void SetDespawnTimer(float duration)
    {
        despawnEndTime = Time.time + duration;
        isDead = true;
    }

}
