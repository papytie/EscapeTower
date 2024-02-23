using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] BonusStatTest bonusTest;
    [SerializeField] List<BonusStatTest> bonusTestList = new();

    [SerializeField] float moveSpeed = 1;
    [SerializeField] float lifePoints = 10;
    [SerializeField] float dashRange = 1;
    [SerializeField] float dashCooldown = 1;
    [SerializeField] float attackCooldown = 1;
    [SerializeField] float defaultValue = 100;

    private void Update()
    {
        //Check for MoveSpeed Bonus
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (CheckForBonus(StatToUpgrade.MoveSpeed, out float improvedStat))
            {
                Debug.Log(StatToUpgrade.MoveSpeed + " modified value is : " + improvedStat);
            }
        }

        //Check for LifePoints Bonus
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            if (CheckForBonusInList(StatToUpgrade.LifePoints, out float improvedStat))
            {
                Debug.Log(StatToUpgrade.LifePoints + " modified value is : " + improvedStat);
            }

        }

    }

    public bool CheckForBonusInList(StatToUpgrade targetStat, out float improvedStat)
    {
        int bonusCount = 0;
        float currentStatValue = GetStatRef(targetStat);

        Debug.Log("searching for " + targetStat + ", raw value : " + currentStatValue);

        foreach (BonusStatTest statBonus in bonusTestList) 
        {
            if (statBonus.Stat == targetStat)
            {
                bonusCount++;
                currentStatValue = StatCalcul(statBonus.Type, currentStatValue, statBonus.Value, statBonus.Stacks);
                Debug.Log(statBonus.Stat + " found! Type : " + statBonus.Type + ", value : " + statBonus.Value + ", stacks : " + statBonus.Stacks);
            }

            else 
                Debug.Log("Bonus type is not valid : " + statBonus.Stat);
        }

        improvedStat = currentStatValue;

        Debug.Log(bonusCount + " Bonus found in total");

        if (bonusCount > 0) 
            return true;
        else 
            return false;
    }

    public bool CheckForBonus(StatToUpgrade targetStat, out float improvedStat)
    {
        Debug.Log("searching for " + targetStat);

        if (bonusTest.Stat == targetStat)
        {
            improvedStat = StatCalcul(bonusTest.Type, GetStatRef(targetStat), bonusTest.Value, bonusTest.Stacks);
            Debug.Log(bonusTest.Stat + " found! Type : " + bonusTest.Type + ", value : " + bonusTest.Value + ", stacks : " + bonusTest.Stacks);
            return true;
        }
        
        improvedStat = GetStatRef(targetStat);
        return false;
    }


    float StatCalcul(BonusType type, float stat, float bonusValue, int bonusStacks)
    {
        switch(type)
        {
            case BonusType.None:
                return stat;
            case BonusType.Add:
                return stat + bonusValue * bonusStacks;
            case BonusType.Subtract:
                return stat - bonusValue * bonusStacks;
            case BonusType.Multiply:
                return stat * bonusValue * bonusStacks;
            case BonusType.Divide:
                return stat / bonusValue * bonusStacks;
            default:
                return stat;
        }
    }

    ref float GetStatRef(StatToUpgrade targetStat)
    {
        switch (targetStat) 
        {
            case StatToUpgrade.MoveSpeed: 
                return ref moveSpeed;
            case StatToUpgrade.LifePoints:
                return ref lifePoints;
            case StatToUpgrade.DashRange:
                return ref dashRange;
            case StatToUpgrade.DashCooldown:
                return ref dashCooldown;
            case StatToUpgrade.AttackCooldown:
                return ref attackCooldown;
            default:
                return ref defaultValue;
        }
    }

}

public enum BonusType
{
    None = 0,
    Add = 1,
    Subtract = 2,
    Multiply = 3,
    Divide = 4
}

public enum StatToUpgrade
{
    None = 0,
    MoveSpeed = 1,
    LifePoints = 2,
    DashRange = 3,
    DashCooldown = 4,
    AttackCooldown = 5
}
