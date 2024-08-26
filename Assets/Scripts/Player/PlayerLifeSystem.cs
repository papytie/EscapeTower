using System;
using UnityEngine;

public class PlayerLifeSystem : MonoBehaviour, ILifeSystem
{
    public bool IsDead => isDead;
    public bool IsInvincible { get => isInvincible; set { isInvincible = value; } }
    public float CurrentLifePoints => currentLifePoints;
    public float MaxLifePoints => maxLifePoints;

    [SerializeField] float currentLifePoints = 10;
    [SerializeField] float maxLifePoints = 20;
    [SerializeField] float InvincibilityDuration = .2f;
    [SerializeField] bool isInvincible = false;

    float invincibilityEndTime = 0;
    bool isDead = false;

    PlayerController controller;

    public void InitRef(PlayerController playerController)
    {
        controller = playerController;
    }

    void Update()
    {
        if (isInvincible && Time.time >= invincibilityEndTime)
        {
            isInvincible = false;
            controller.Animator.SetBool(GameParams.Animation.PLAYER_INVINCIBILITY_BOOL, false);
        }
    }

    public void TakeDamage(float damageValue, Vector2 normal)
    {
        if (isDead || isInvincible) return;

        StartInvincibility(InvincibilityDuration);
        currentLifePoints -= damageValue;
        if (currentLifePoints <= 0) 
        {
            currentLifePoints = 0;
            isDead = true;
            controller.Animator.SetTrigger(GameParams.Animation.PLAYER_DIE_TRIGGER);
            return;
        }
        controller.Bump.BumpedAwayActivation(-normal, damageValue);

        controller.Animator.SetTrigger(GameParams.Animation.PLAYER_TAKEDAMAGE_TRIGGER);
    }

    public void HealUp(float healValue)
    {
        if (isDead) return;

        currentLifePoints = Mathf.Min(currentLifePoints + healValue, maxLifePoints);
    }

    public void StartInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityEndTime = MathF.Max(invincibilityEndTime, Time.time + duration);
        controller.Animator.SetBool(GameParams.Animation.PLAYER_INVINCIBILITY_BOOL, true);
    }

}
