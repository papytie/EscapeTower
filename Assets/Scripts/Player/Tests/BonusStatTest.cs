using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BonusStatTest : MonoBehaviour, IBonus
{
    [SerializeField] StatToUpgrade stat;
    [SerializeField] BonusType type;
    [SerializeField] float value;
    [SerializeField] int stacks;

    public StatToUpgrade Stat => stat;
    public BonusType Type => type;
    public float Value => value;
    public int Stacks => stacks;
}

public interface IBonus
{
    StatToUpgrade Stat { get; }
    BonusType Type { get; }
    float Value { get; }
    int Stacks { get; }
}


