public interface ILifeSystem
{
    public bool IsDead { get; }
    public float CurrentLifePoints { get; }
    public float MaxLifePoints { get; }
    public void TakeDamage(float damage);
    public void HealUp(float heal);
}
