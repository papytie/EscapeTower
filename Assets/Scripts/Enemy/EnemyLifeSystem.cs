using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameParams;

public class EnemyLifeSystem : MonoBehaviour, ILifeSystem
{
    public bool IsDead => isDead;
    public float CurrentLifePoints => currentLifePoints;
    public float MaxLifePoints => maxLifePoints;

    [Header("Life Settings")]
    [SerializeField] float currentLifePoints = 10;
    [SerializeField] float maxLifePoints = 20;
    [SerializeField] float despawnDuration = 3f;

    bool isDead = false;
    float despawnEndTime;

    Animator animator;
    Collider2D enemyCollider;

    public void InitRef(Animator animatorRef)
    {
        animator = animatorRef;
    }

    private void Start()
    {
        enemyCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isDead && Time.time >= despawnEndTime)
            Destroy(gameObject);
    }

    public void TakeDamage(float damageValue)
    {
        if (isDead) return;

        currentLifePoints -= damageValue;
        if (currentLifePoints <= 0)
        {
            currentLifePoints = 0;
            SetDespawnTimer(despawnDuration);
            enemyCollider.enabled = false;
            animator.SetBool(GameParams.Animation.ENEMY_DIE_BOOL, true);
            return;
        }
        animator.SetTrigger(GameParams.Animation.ENEMY_TAKEDAMAGE_TRIGGER);
    }

    public void HealUp(int healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

    void SetDespawnTimer(float duration)
    {
        despawnEndTime = Time.time + duration;
        isDead = true;
    }

    public void HealUp(float heal)
    {
        throw new System.NotImplementedException();
    }
}
