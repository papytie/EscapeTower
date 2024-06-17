using UnityEngine;

public interface IAttack
{
    public bool IsAttacking { get; }
    public bool IsOnCooldown { get; }
    public bool IsOnAttackLag { get; }
    public float AttackRange { get; }
    public void Init(IAttackData data, EnemyStatsComponent statsComponent, Animator animatorRef);
    public void AttackActivation();
}
