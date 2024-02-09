using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int BaseDamage => baseDamage;

    [SerializeField] int baseDamage = 1;
}
