using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]

public class PlayerWeapon : MonoBehaviour
{
    public int Damage => damage;
    public float Cooldown => cooldown;
    public float Lag => lag;

    [Header("Weapon Settings")]
    [SerializeField] Animator weaponAnimator;
    [SerializeField] int damage = 1;
    [SerializeField] float cooldown = .5f;
    [SerializeField] float lag = .2f;

    private void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    public void WeaponAttackFX()
    {
        weaponAnimator.SetTrigger(GameParams.Animation.WEAPON_ATTACK_TRIGGER);
    }


}
