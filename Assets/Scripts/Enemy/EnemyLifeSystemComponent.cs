using System;
using UnityEngine;

public class EnemyLifeSystemComponent : MonoBehaviour, ILifeSystem
{
    public bool IsDead => isDead;

    public bool IsInvincible { get => isDead; set { isDead = value; } }

    bool isDead = false;

    public event Action OnDeath = null;
    public event Action OnTakeDamage = null;

    EnemyController controller;

    public void InitRef(EnemyController ctrlRef)
    {
        controller = ctrlRef;
    }

    public void TakeDamage(float damageValue, Vector2 atkVector)
    {
        if (isDead) return;

        controller.Stats.CurrentHealth -= damageValue;
        controller.Stats.LastDMGReceived = damageValue;
        controller.Stats.LastATKNormalReceived = atkVector;

        if (controller.Stats.CurrentHealth <= 0)
        {
            controller.Stats.CurrentHealth = 0;
            controller.CircleCollider.enabled = false;
            isDead = true;
            OnDeath?.Invoke();
            return;
        }
        else OnTakeDamage?.Invoke();
    }

    public void HealUp(float healValue)
    {
        if (isDead) return;
        controller.Stats.CurrentHealth = Mathf.Min(controller.Stats.CurrentHealth + healValue, controller.Stats.MaxHealth);
    }
}
