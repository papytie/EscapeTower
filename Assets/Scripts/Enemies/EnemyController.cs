using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(EnemyLifeSystem), typeof(EnemyAttack))]

public class EnemyController : MonoBehaviour
{
    Animator animator;
    EnemyLifeSystem lifeSystem;
    EnemyAttack attack;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lifeSystem = GetComponent<EnemyLifeSystem>();
        attack = GetComponent<EnemyAttack>();
    }

    void Start()
    {
        lifeSystem.InitRef(animator);
    }
}
