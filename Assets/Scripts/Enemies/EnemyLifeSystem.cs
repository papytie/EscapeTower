using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameParams;

public class EnemyLifeSystem : MonoBehaviour
{
    public bool IsDead => isDead;
    public int CurrentLifePoints => currentLifePoints;
    public int MaxLifePoints => maxLifePoints;

    [SerializeField] int currentLifePoints = 10;
    [SerializeField] int maxLifePoints = 20;

    bool isDead = false;

    Animator animator;

    public void InitRef(Animator animatorRef)
    {
        animator = animatorRef;
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

    public void HealUp(int healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

}
