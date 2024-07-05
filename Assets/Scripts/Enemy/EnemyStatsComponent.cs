using UnityEngine;

public class EnemyStatsComponent : MonoBehaviour
{
    public float MoveSpeed => moveSpeed * ScalingFactor;
    public float CollisionDamage => collisionDamage * ScalingFactor;
    public float MeleeDamage => meleeDamage * ScalingFactor;
    public float MeleeRange => meleeRange * ScalingFactor;
    public float MeleeCooldown => meleeCooldown / ScalingFactor;
    public float ProjectileDamage => projectileDamage * ScalingFactor;
    public float ProjectileRange => projectileRange * ScalingFactor;
    public float ProjectileCooldown => projectileCooldown / ScalingFactor;
    public float DetectionRadius => detectionRadius * ScalingFactor;
    public float MaxHealth => maxHealth * ScalingFactor;
    public float Weight => weight * ScalingFactor;
    public float RecupTime => recupTime / ScalingFactor;
    public float ReactionTime => reactionTime / ScalingFactor;
    
    public float ScalingFactor { get; set; }

    public float CurrentHealth { get; set; }
    public bool IsDead => CurrentHealth <= 0;
    public float LastDMGReceived { get; set; }
    public Vector3 LastATKNormalReceived { get; set; }

    [Header("Stats"), Space]
    [Header("General")]
    [SerializeField] float maxHealth = 10;
    [SerializeField] float recupTime = 1;
    [SerializeField] float reactionTime = 1;
    [SerializeField] float collisionDamage = 1;
    [SerializeField] float detectionRadius = 1;
    [SerializeField] float weight = 1;
    [Header("Movement")]
    [SerializeField] float moveSpeed = 1;
    [Header("Melee")]
    [SerializeField] float meleeDamage = 1;
    [SerializeField] float meleeRange = 1;
    [SerializeField] float meleeCooldown = 1;
    [Header("Ranged")]
    [SerializeField] float projectileDamage = 1;
    [SerializeField] float projectileRange = 1;
    [SerializeField] float projectileCooldown = 1;

    public void Init()
    {
        CurrentHealth = maxHealth;
        ScalingFactor = 1;
    }

    public void SetScalingFactorTo(float value)
    {
        ScalingFactor = value;
    }
}
