using UnityEngine;

public class EnemyLifeSystemComponent : MonoBehaviour, ILifeSystem
{
    public bool IsDead => controller.Stats.IsDead;

    EnemyController controller;

    public void InitRef(EnemyController ctrlRef)
    {
        controller = ctrlRef;
    }

    public void TakeDamage(float damageValue, Vector2 atkVector)
    {
        if (controller.Stats.IsDead) return;

        controller.Stats.CurrentHealth -= damageValue;
        controller.Stats.LastDMGReceived = damageValue;
        controller.Stats.LastATKNormalReceived = atkVector;

        if (controller.Stats.CurrentHealth <= 0)
        {
            controller.Stats.CurrentHealth = 0;
            controller.CircleCollider.enabled = false;
            controller.MainBehaviour.SetDieState();
            return;
        }
        else controller.MainBehaviour.SetTakeDamageState();
    }

    public void HealUp(float healValue)
    {
        if (controller.Stats.IsDead) return;
        controller.Stats.CurrentHealth = Mathf.Min(controller.Stats.CurrentHealth + healValue, controller.Stats.MaxHealth);
    }
}
