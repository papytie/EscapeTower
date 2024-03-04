using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameParams;

public class EnemyLifeSystem : MonoBehaviour
{
    public bool IsDespawning => isDespawning;

    [Header("Life Settings")]
    [SerializeField] float currentLifePoints = 10;
    [SerializeField] float maxLifePoints = 20;
    [SerializeField] float despawnDuration = 3f;

    bool isDespawning = false;
    float despawnEndTime;

    Animator animator;

    public void InitRef(Animator animatorRef)
    {
        animator = animatorRef;
    }

    private void Update()
    {
        if (isDespawning && Time.time >= despawnEndTime)
            Destroy(gameObject);
    }

    public void TakeDamage(float damageValue)
    {
        if (isDespawning) return;

        currentLifePoints -= damageValue;
        if (currentLifePoints <= 0)
        {
            currentLifePoints = 0;
            SetDespawnTimer(despawnDuration);
            animator.SetBool(GameParams.Animation.ENEMY_DIE_BOOL, true);
            return;
        }
        animator.SetTrigger(GameParams.Animation.ENEMY_TAKEDAMAGE_TRIGGER);
    }

    public void HealUp(int healValue)
    {
        if (isDespawning) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

    void SetDespawnTimer(float duration)
    {
        despawnEndTime = Time.time + duration;
        isDespawning = true;
    }

}
