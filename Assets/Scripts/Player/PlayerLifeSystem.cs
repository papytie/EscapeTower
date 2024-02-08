using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeSystem : MonoBehaviour
{
    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible;
    public int CurrentLifePoints => currentLifePoints;
    public int MaxLifePoints => maxLifePoints;

    [SerializeField] int currentLifePoints = 10;
    [SerializeField] int maxLifePoints = 20;
    [SerializeField] float InvincibilityTime = .2f;

    Animator animator;
    float currentInvincibilityTime = 0;
    bool isDead = false;
    bool isInvincible = false;

    void Update()
    {
        if (isInvincible)
            InvincibilityTimer();
    }

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
            animator.SetBool(GameParams.Animation.PLAYER_DIE_BOOL, true);
            return;
        }
        animator.SetTrigger(GameParams.Animation.PLAYER_TAKEDAMAGE_TRIGGER);
        isInvincible = true;
    }

    public void HealUp(int healValue)
    {
        if (isDead) return;

        currentLifePoints += healValue;
        if (currentLifePoints > maxLifePoints) 
        {
            currentLifePoints = maxLifePoints;
        }
    }

    void InvincibilityTimer()
    {
        currentInvincibilityTime += Time.deltaTime;
        if (currentInvincibilityTime < InvincibilityTime)
        {
            currentInvincibilityTime = 0;
            isInvincible = false;
        }
    }


}
