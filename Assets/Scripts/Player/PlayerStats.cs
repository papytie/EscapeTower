using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public IReadOnlyDictionary<StatModifierTemplate, int> CurrentBonuses { get; private set; }

    float defaultValue = 0;
    
    Dictionary<StatModifierTemplate, int> playerBonusList = new();

    PlayerController controller;

    public void InitRef(PlayerController playerController)
    {
        controller = playerController;
        CurrentBonuses = new ReadOnlyDictionary<StatModifierTemplate, int>(playerBonusList);
    }

    public void AddBonus(StatModifierTemplate targetStat)
    {
        if (playerBonusList.ContainsKey(targetStat))
        {
            playerBonusList[targetStat] += 1;
            Debug.Log(targetStat.name + " total stacks = " + playerBonusList[targetStat]);
        }
        else
        {
            playerBonusList.Add(targetStat, 1);
            Debug.Log("New" + targetStat.name + " added, give bonus to : " + targetStat.MainStat);
        }
    }

    public float GetModifiedMainStat(MainStat mainStat)
    {
        float statValue = GetBaseMainStatValue(mainStat);

        foreach (KeyValuePair<StatModifierTemplate, int> bonusStack in playerBonusList) 
        {
            if (bonusStack.Key.MainStat == mainStat)
                statValue = ReturnOperationResult(statValue, bonusStack.Key.ModifValue, bonusStack.Value, bonusStack.Key.Calcul, bonusStack.Key.ValueType);
        }
        return statValue;
    }

    public float GetModifiedSecondaryStat(SecondaryStat secondaryStat)
    {
        float statValue = GetBaseSecondaryStatValue(secondaryStat);

        return secondaryStat switch
        {
            SecondaryStat.AttackCooldownDuration => Mathf.Max(statValue / GetModifiedMainStat(MainStat.AttackSpeed), 0),
            SecondaryStat.AttackLagDuration => Mathf.Max(statValue / GetModifiedMainStat(MainStat.AttackSpeed), 0),
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
            MainStat.MoveSpeed => controller.Movement.MoveSpeed,
            MainStat.Damage => controller.WeaponSlot.EquippedWeapon.AttackData.damage,
            MainStat.AttackSpeed => 1,
            MainStat.DashSpeed => 1,
            MainStat.DashDistance => controller.Dash.Distance,
            MainStat.ProjectileNumber => controller.WeaponSlot.EquippedWeapon.AttackData.projectileData.spawnNumber,
            MainStat.ProjectileRange => controller.WeaponSlot.EquippedWeapon.AttackData.projectileData.range,
            _ => defaultValue,
        };
    }

    float GetBaseSecondaryStatValue(SecondaryStat targetStat)
    {
        return targetStat switch
        {   SecondaryStat.AttackCooldownDuration => controller.WeaponSlot.EquippedWeapon.AttackData.cooldown,
            SecondaryStat.AttackLagDuration => controller.WeaponSlot.EquippedWeapon.AttackData.lag,
            SecondaryStat.HitboxDuration => controller.WeaponSlot.EquippedWeapon.AttackData.hitboxDuration,
            SecondaryStat.DashCooldown => controller.Dash.Cooldown,
            SecondaryStat.DashDuration => controller.Dash.Duration,
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
    ProjectileNumber = 5,
    ProjectileRange = 6,
}

public enum SecondaryStat
{
    AttackCooldownDuration = 0,
    AttackLagDuration = 1,
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