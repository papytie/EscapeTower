using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsComponent : MonoBehaviour
{
    public float MoveSpeed => moveSpeed * scalingFactor;
    public float AttackSpeed => attackSpeed / scalingFactor;
    public float CollisionDamage => collisionDamage * scalingFactor;
    public float MeleeDamage => meleeDamage * scalingFactor;
    public float ProjectileDamage => projectileDamage * scalingFactor;
    public float DetectionRadius => detectionRadius * scalingFactor;
    public float MaxLifePoints => maxLifePoints * scalingFactor;

    [Header("Stats"), Space]
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float attackSpeed = 1;
    [SerializeField] float collisionDamage = 1;
    [SerializeField] float meleeDamage = 1;
    [SerializeField] float projectileDamage = 1;
    [SerializeField] float detectionRadius = 1;
    [SerializeField] float maxLifePoints = 10;
    [SerializeField] float scalingFactor = 1;

    public void SetScalingFactorTo(float value)
    {
        scalingFactor = value;
    }
}
