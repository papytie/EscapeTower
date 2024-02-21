using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameParams;

public class EnemyLifeSystem : MonoBehaviour
{
    public bool IsDead => isDead;
    public int CurrentLifePoints => currentLifePoints;
    public int MaxLifePoints => maxLifePoints;

    [Header("Life Settings")]
    [SerializeField] int currentLifePoints = 10;
    [SerializeField] int maxLifePoints = 20;
    [SerializeField] float despawnTime = 3f;

    bool isDead = false;
    float currentTime;

    Animator animator;

    public void InitRef(Animator animatorRef)
    {
        animator = animatorRef;
    }

    private void Update()
    {
        if(isDead)
            DespawnTimer();
    }

    public void TakeDamage(int damageValue)
    {
        if (isDead) return;

        currentLifePoints -= damageValue;
        if (currentLifePoints <= 0)
        {
            currentLifePoints = 0;
            isDead = true;
            animator.SetBool(GameParams.Animation.ENEMY_DIE_BOOL, true);
            return;
        }
        animator.SetTrigger(GameParams.Animation.ENEMY_TAKEDAMAGE_TRIGGER);
    }

    void DespawnTimer()
    {
        currentTime += Time.deltaTime;
        if (currentTime > despawnTime) 
        {
            Destroy(gameObject);
        }
    }

    public void HealUp(int healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

}
