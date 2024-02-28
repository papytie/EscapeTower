using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerStats : MonoBehaviour
{
    PlayerMovement movement;
    PlayerWeaponSlot weaponSlot;

    float defaultValue;
    
    Dictionary<StatModifierTemplate, int> playerBonusList = new Dictionary<StatModifierTemplate, int>();

    public void InitRef(PlayerMovement movementRef, PlayerWeaponSlot weaponSlotRef)
    {
        movement = movementRef;
        weaponSlot = weaponSlotRef;

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
        Debug.Log("New" + targetStat.name + " added, give bonus to : " + targetStat.Stat);
        playerBonusList.Add(targetStat, 1);
    }

    public float GetUpdatedStat(StatConcerned targetStat)
    {
        float baseStatValue = GetBaseStat(targetStat);

        foreach (KeyValuePair<StatModifierTemplate, int> keyPair in playerBonusList) 
        {
            if (keyPair.Key.Stat == targetStat)
            {
                playerBonusList.TryGetValue(keyPair.Key, out int value);
                baseStatValue += keyPair.Key.ModifValue * value;
            }
        }
        return baseStatValue;
    }

    float GetBaseStat(StatConcerned targetStat)
    {
        switch (targetStat) 
        {
            case StatConcerned.MoveSpeed: 
                return movement.MoveSpeed;
            case StatConcerned.Damage:
                return weaponSlot.EquippedWeapon.Damage;
            default:
                return defaultValue;
        }
    }

}



