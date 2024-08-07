using UnityEngine;

public class EnemyStatsComponent : MonoBehaviour
{
    public float MoveSpeed => baseMoveSpeed * ScalingFactor;
    public float CollisionDamage => collisionDamage * ScalingFactor;
    public float DetectionRadius => detectionRadius * ScalingFactor;
    public float MaxHealth => maxHealth * ScalingFactor;
    public float Weight => weight * ScalingFactor;
    
    public float ScalingFactor { get; set; }

    public float CurrentHealth { get; set; }
    public bool IsDead => CurrentHealth <= 0;
    public float LastDMGReceived { get; set; }
    public Vector3 LastATKNormalReceived { get; set; }

    [Header("Stats"), Space]
    [Header("General")]
    [SerializeField] float maxHealth = 10;
    [SerializeField] float collisionDamage = 1;
    [SerializeField] float detectionRadius = 1;
    [SerializeField] float weight = 1;
    [Header("Movement")]
    [SerializeField] float baseMoveSpeed = 1;

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
