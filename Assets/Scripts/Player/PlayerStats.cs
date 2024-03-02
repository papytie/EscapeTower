using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public IReadOnlyDictionary<StatModifierTemplate, int> CurrentBonuses { get; private set; }

    PlayerMovement movement;
    PlayerWeaponSlot weaponSlot;
    PlayerDash dash;
    PlayerAttack attack;

    float defaultValue = 0;
    
    Dictionary<StatModifierTemplate, int> playerBonusList = new();

    public void InitRef(PlayerMovement movementRef, PlayerWeaponSlot weaponSlotRef, PlayerDash dashRef, PlayerAttack attackRef)
    {
        CurrentBonuses = new ReadOnlyDictionary<StatModifierTemplate, int>(playerBonusList);
        movement = movementRef;
        weaponSlot = weaponSlotRef;
        dash = dashRef;
        attack = attackRef;
    }

    public void AddBonus(StatModifierTemplate targetStat)
    {
        if (playerBonusList.TryGetValue(targetStat, out int value))
        {
            value++;
            playerBonusList[targetStat] = value;
            Debug.Log(targetStat.name + " total stacks = " + value);
            return;
        }
        Debug.Log("New" + targetStat.name + " added, give bonus to : " + targetStat.MainStat);
        playerBonusList.Add(targetStat, 1);
    }

    public float GetModifiedMainStat(MainStat mainStat)
    {
        float statValue = GetBaseMainStatValue(mainStat);

        foreach (KeyValuePair<StatModifierTemplate, int> bonusID in playerBonusList) 
        {
            if (bonusID.Key.MainStat == mainStat)
            {
                playerBonusList.TryGetValue(bonusID.Key, out int bonusStack);

                //Calcul final value depending on Calcul Type and number of stacks
                return ReturnOperationResult(statValue, bonusID.Key.ModifValue, bonusStack, bonusID.Key.Calcul, bonusID.Key.ValueType);
            }
        }
        return statValue;
    }

    public float GetModifiedSecondaryStat(SecondaryStat secondaryStat)
    {
        float statValue = GetBaseSecondaryStatValue(secondaryStat);

        return secondaryStat switch
        {
            SecondaryStat.AttackCooldown => Mathf.Max(statValue / GetModifiedMainStat(MainStat.AttackSpeed), 0),
            SecondaryStat.AttackLag => Mathf.Max(statValue / GetModifiedMainStat(MainStat.AttackSpeed), 0),
            SecondaryStat.HitboxDuration => Mathf.Max(statValue / GetModifiedMainStat(MainStat.AttackSpeed), 0),
            SecondaryStat.DashCooldown => Mathf.Max(statValue / GetModifiedMainStat(MainStat.DashSpeed), 0),
            SecondaryStat.DashDuration => Mathf.Max(statValue / GetModifiedMainStat(MainStat.DashSpeed), 0),
            _ => statValue,
        };
    }

    float ReturnOperationResult(float statValue, float bonusValue, int bonusStack, CalculType calculType, ValueType valueType)
    {
        return valueType switch
        {
            ValueType.Flat => calculType switch
            {
                CalculType.Add => statValue += bonusValue * bonusStack,
                CalculType.Subtract => statValue -= bonusValue * bonusStack,
                CalculType.Multiply => statValue *= (bonusValue * bonusStack),
                CalculType.Divide => statValue /= (bonusValue * bonusStack),
                _ => statValue,
            },

            ValueType.Percent => calculType switch
            {
                CalculType.Add => statValue += statValue * (bonusValue/100) * bonusStack,
                CalculType.Subtract => statValue -= statValue * (bonusValue/100) * bonusStack,
                CalculType.Multiply => statValue *= ((bonusValue/100) * bonusStack),
                CalculType.Divide => statValue /= ((bonusValue/100) * bonusStack),
                _ => statValue,
            },
            _ => statValue,
        };
    }

    float GetBaseMainStatValue(MainStat targetStat)
    {
        return targetStat switch
        {
            MainStat.MoveSpeed => movement.MoveSpeed,
            MainStat.Damage => weaponSlot.EquippedWeapon.Damage,
            MainStat.AttackSpeed => attack.AttackSpeed,
            MainStat.DashSpeed => dash.Speed,
            MainStat.DashDistance => dash.Distance,
            _ => defaultValue,
        };
    }

    float GetBaseSecondaryStatValue(SecondaryStat targetStat)
    {
        return targetStat switch
        {   SecondaryStat.AttackCooldown => weaponSlot.EquippedWeapon.Cooldown,
            SecondaryStat.AttackLag => weaponSlot.EquippedWeapon.Lag,
            SecondaryStat.HitboxDuration => weaponSlot.EquippedWeapon.HitboxDuration,
            SecondaryStat.DashCooldown => dash.Cooldown,
            SecondaryStat.DashDuration => dash.Duration,
            _ => defaultValue,
        };
    }

}

public enum MainStat
{
    MoveSpeed = 0,
    Damage = 1,
    AttackSpeed = 2,
    DashSpeed = 3,
    DashDistance = 4,
}

public enum SecondaryStat
{
    AttackCooldown = 0,
    AttackLag = 1,
    DashCooldown = 2,
    DashDuration = 3,
    HitboxDuration = 4,
}

public enum ValueType
{
    Flat = 0,
    Percent = 1,
}

public enum CalculType
{
    Add = 0,
    Subtract = 1,
    Multiply = 2,
    Divide = 3,
}

public enum SecondaryStatBehavior
{
    Same = 0,
    Invert = 1,
}