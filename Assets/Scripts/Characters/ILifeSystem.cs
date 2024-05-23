using UnityEngine;

public interface ILifeSystem
{
    public bool IsDead { get; }
    public float CurrentLifePoints { get; }
    public float MaxLifePoints { get; }
    public void TakeDamage(float damage, Vector2 normal);
    public void HealUp(float heal);
}
